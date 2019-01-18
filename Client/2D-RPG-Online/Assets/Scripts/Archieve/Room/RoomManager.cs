﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : Menu {

    #region Singleton

    public static RoomManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Action onRoomCreated;
    public Action onRoomJoined;


    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private Button _btnStartGame;
    [SerializeField]
    private Button _btnReady;
    [SerializeField]
    private Button _btnNotReady;
    [SerializeField]
    private Button _btnLeave;

    private List<PlayerController> _otherPlayerControllers = new List<PlayerController>();

    private PlayerController _myPlayerController;

    private RoomPlayerInfo _leaderPlayerInfo;
    private FixedJoystick _joystick;
    private Button _btnAttack;

    public int serverTickrate;

    private void Start() {
        NetworkManager.instance.onGameplayServerConnectionSuccess += OnGameplayServerConnectionSuccess;

        NetworkManager.mss.AddEventListener(MSPlayerEvent.RoomUpdate, OnRoomUpdated);

        NetworkManager.mss.AddEventListener(MSPlayerEvent.CreatePlayer, OnPlayerCreated);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoin, OnRoomJoinSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoinFailed, OnRoomJoinFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreate, OnRoomCreated);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreateFailed, OnRoomCreateFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDelete, OnRoomDeleted);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDeleteFailed, OnRoomDeleteFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerJoined, OnRoomPlayerJoined);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerLeft, OnRoomPlayerLeft);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeave, OnRoomLeaveSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeaveFailed, OnRoomLeaveFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomGetPlayers, OnRoomGetPlayers);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomChangeLeader, OnRoomLeaderChanged);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatus, OnPlayerReadyStatusChanged);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatusFailed, OnPlayerReadyStatusChangeFailed);
    }

    private void Update() {
        var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        var renderTimestamp = now - (1000.0 / serverTickrate);

        for (int ii = 0; ii < _otherPlayerControllers.Count; ii++) {
            PlayerController entity = _otherPlayerControllers[ii];

            // Drop older positions.
            while (entity.PositionBuffer.Count >= 2 && entity.PositionBuffer[1].updateTime <= renderTimestamp) {
                entity.PositionBuffer = entity.PositionBuffer.Skip(1).ToList();
            }

            // Interpolate between the two surrounding authoritative positions.
            if (entity.PositionBuffer.Count >= 2 && entity.PositionBuffer[0].updateTime <= renderTimestamp && renderTimestamp <= entity.PositionBuffer[1].updateTime) {
                Vector2 firstVector = entity.PositionBuffer[0].vector2;
                Vector2 secondVector = entity.PositionBuffer[1].vector2;

                double t0 = entity.PositionBuffer[0].updateTime;
                double t1 = entity.PositionBuffer[1].updateTime;
                
                double interpX = firstVector.x + (secondVector.x - firstVector.x) * (renderTimestamp - t0) / (t1 - t0);
                double interpY = firstVector.y + (secondVector.y - firstVector.y) * (renderTimestamp - t0) / (t1 - t0);

                Vector2 newPosition = new Vector2((float)interpX, (float)interpY);

                entity.ToNewPosition(newPosition);
            }
        }
    }

    public void Initialize() {
        if (GameObject.Find("DummyCamera") != null) {
            GameObject.Find("DummyCamera").SetActive(false);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CreateRoom() {
        StartCoroutine(ICreateRoom());
    }

    public IEnumerator ICreateRoom() {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.Room = new MSSRoom();

        roomData.Room.Name = CharacterManager.instance.SelectedCharacter.name + "\'s Room";
        roomData.Room.IsPrivate = false;
        roomData.Room.MaxUserCount = 1000;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    public void JoinRoom() {
        StartCoroutine(IJoinRoom("banaismailde"));
    }

    public void LeaveRoom() {
        Debug.Log("Attemp to leave room!");
        NetworkManager.mss.SendMessage(MSServerEvent.RoomLeave);
    }

    public void DeleteRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.Room = new MSSRoom();
        roomData.Room.Id = NetworkManager.mss.JoinedRoom.Id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomDelete, data);
    }

    public void SetReadyStatus(RoomClientSlot slot) {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.PlayerReadyStatusInfo.IsReady = !slot.IsReady;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomPlayerReadyStatus, data);
    }

    private void CreateMyPlayer(RoomPlayerInfo playerInfo) {
        GameObject player = Instantiate(_playerPrefab, new Vector2(playerInfo.CurrentGObject.PosX, playerInfo.CurrentGObject.PosY), Quaternion.identity);

        _myPlayerController = player.GetComponent<PlayerController>();
        _myPlayerController.Initialize(playerInfo.CurrentGObject);
    }


    private void CreatePlayer(RoomPlayerInfo playerInfo) {
        PlayerObject tempCurrentObject = playerInfo.CurrentGObject;

        playerInfo.CurrentGObject = new PlayerObject();
        playerInfo.CurrentGObject = tempCurrentObject;

        PlayerController playerController = Instantiate(_playerPrefab, new Vector2(playerInfo.CurrentGObject.PosX, playerInfo.CurrentGObject.PosY), Quaternion.identity).GetComponent<PlayerController>();
        playerController.Initialize(playerInfo.CurrentGObject);

        _otherPlayerControllers.Add(playerController);
    }

    private IEnumerator IJoinRoom(string id) {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.Room = new MSSRoom();
        roomData.Room.Id = "123";

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    private bool OnGameplayServerConnectionSuccess() {
        return true;
    }

    private void OnRoomUpdated(ShiftServerData data) {
        serverTickrate = data.SvTickRate;

        for (int ii = 0; ii < data.GoUpdatePacket.PlayerList.Count; ii++) {
            PlayerObject updatedPlayerObject = data.GoUpdatePacket.PlayerList[ii];

            //Reconciliation
            if (NetworkManager.instance.Reconciliaton) {
                if (updatedPlayerObject.Oid == _myPlayerController.Oid) {
                    for (int jj = 0; jj < _myPlayerController.PlayerInputs.Count; jj++) {
                        if (_myPlayerController.GetSequenceID(jj) <= updatedPlayerObject.LastProcessedSequenceID) {
                            _myPlayerController.RemoveRange(jj, 1);
                        }
                    }
                }
            } else {
                _myPlayerController.ClearPlayerInputs();
            }

            //Movement
            for (int jj = 0; jj < _otherPlayerControllers.Count; jj++) {
                if (_otherPlayerControllers[jj].Oid == updatedPlayerObject.Oid) {
                    Vector3 updatedPosition = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);

                    DateTime updateTime = DateTime.UtcNow;
                    var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                    _otherPlayerControllers[jj].AddPositionToBuffer(now, updatedPosition);
                }
            }
        }
    }

    private void OnPlayerCreated(ShiftServerData data) {
        Debug.Log("OnPlayerCreated: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        if (playerInfo.CurrentGObject.Name == AccountManager.instance.SelectedCharacterName) {
            CreateMyPlayer(playerInfo);
        } else {
            CreatePlayer(playerInfo);
        }
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        Debug.Log("OnRoomJoinSuccess: " + data);

        MSSRoom joinedRoom = data.RoomData.Room;

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            CreatePlayer(data.RoomData.PlayerList[ii]);
        }

        onRoomJoined?.Invoke();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        Debug.Log("OnRoomJoinFailed: " + data);
    }

    private void OnRoomCreated(ShiftServerData data) {
        Debug.Log("OnRoomCreated: " + data);

        //playerInfo = data.RoomData.CreatedRoom.Teams;

        onRoomCreated?.Invoke();
    }

    private void OnRoomCreateFailed(ShiftServerData data) {
        Debug.Log("OnRoomCreateFailed: " + data);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        Debug.Log("OnRoomDeleted: " + data);

        _otherPlayerControllers = new List<PlayerController>();
    }

    private void OnRoomDeleteFailed(ShiftServerData data) {
        Debug.Log("OnRoomDeleteFailed: " + data);
    }

    private void OnRoomPlayerJoined(ShiftServerData data) {
        Debug.Log("OnRoomPlayerJoined: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        CreatePlayer(playerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        Debug.Log("OnRoomPlayerLeft: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        for (int ii = 0; ii < _otherPlayerControllers.Count; ii++) {
            if (playerInfo.CurrentGObject.Oid == _otherPlayerControllers[ii].Oid) {
                _otherPlayerControllers.Remove(_otherPlayerControllers[ii]);
                _otherPlayerControllers[ii].Destroy();
                break;
            }
        }
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        Debug.Log("OnRoomLeaveSuccess: " + data);

        SceneManager.UnloadSceneAsync("Gameplay");

        this.Hide();
        MenuManager.instance.Show();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        Debug.Log("OnRoomLeaveFailed: " + data);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        Debug.Log("OnRoomGetPlayers: " + data);
    }

    private void OnRoomLeaderChanged(ShiftServerData data) {
        Debug.Log("OnRoomLeaderChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;
    }

    private void OnPlayerReadyStatusChanged(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;
    }

    private void OnPlayerReadyStatusChangeFailed(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChangeFailed: " + data);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
        if (scene.name == "Gameplay") {
            Initialize();
        }
    }
}
