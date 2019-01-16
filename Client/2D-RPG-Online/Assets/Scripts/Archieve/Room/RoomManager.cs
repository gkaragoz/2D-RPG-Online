using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private List<RoomPlayerInfo> _otherPlayers = new List<RoomPlayerInfo>();
    private RoomPlayerInfo _myPlayerInfo = new RoomPlayerInfo();
    private PlayerController _myPlayerController;

    private RoomPlayerInfo _leaderPlayerInfo;
    private FixedJoystick _joystick;
    private Button _btnAttack;

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
        roomData.Room.MaxUserCount = 2;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    public void JoinRoom() {
        StartCoroutine(IJoinRoom("123"));
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

    private void CreateMyPlayer(PlayerObject playerObject) {
        GameObject player = Instantiate(_playerPrefab, new Vector2(playerObject.PosX, playerObject.PosY), Quaternion.identity);

        _myPlayerInfo.ObjectId = playerObject.Oid;

        _myPlayerController = player.GetComponent<PlayerController>();
        _myPlayerController.Initialize(_myPlayerInfo);
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
        for (int ii = 0; ii < data.GoUpdatePacket.PlayerList.Count; ii++) {
            Debug.Log(data.GoUpdatePacket.PlayerList[ii]);

            PlayerObject updatedPlayerObject = data.GoUpdatePacket.PlayerList[ii];

            Vector3 shadowPosition = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);
            _myPlayerController.SetShadowPosition(shadowPosition);

            if (NetworkManager.instance.Reconciliaton) {
                if (updatedPlayerObject.Oid == _myPlayerInfo.ObjectId) {
                    for (int jj = 0; jj < _myPlayerController.PlayerInputs.Count; jj++) {
                        if (_myPlayerController.GetSequenceID(jj) <= updatedPlayerObject.LastProcessedSequenceID) {
                            _myPlayerController.RemoveRange(jj, 1);
                        }
                    }
                }
            } else {
                _myPlayerController.ClearPlayerInputs();
            }
        }
    }

    private void OnPlayerCreated(ShiftServerData data) {
        Debug.Log("OnPlayerCreated: " + data);

        PlayerObject playerObject = data.SPlayerObject;

        if (_myPlayerInfo.ObjectId == 0) {
            CreateMyPlayer(playerObject);
        }
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        Debug.Log("OnRoomJoinSuccess: " + data);

        MSSRoom joinedRoom = data.RoomData.Room;

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        _myPlayerInfo = playerInfo;

        for (int ii = 0; ii < _otherPlayers.Count; ii++) {
            _otherPlayers.Add(data.RoomData.PlayerList[ii]);
        }

        Initialize();

        onRoomJoined?.Invoke();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        Debug.Log("OnRoomJoinFailed: " + data);
    }

    private void OnRoomCreated(ShiftServerData data) {
        Debug.Log("OnRoomCreated: " + data);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        _myPlayerInfo = playerInfo;

        //playerInfo = data.RoomData.CreatedRoom.Teams;

        onRoomCreated?.Invoke();
    }

    private void OnRoomCreateFailed(ShiftServerData data) {
        Debug.Log("OnRoomCreateFailed: " + data);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        Debug.Log("OnRoomDeleted: " + data);

        _otherPlayers = new List<RoomPlayerInfo>();
    }

    private void OnRoomDeleteFailed(ShiftServerData data) {
        Debug.Log("OnRoomDeleteFailed: " + data);
    }

    private void OnRoomPlayerJoined(ShiftServerData data) {
        Debug.Log("OnRoomPlayerJoined: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        _otherPlayers.Add(playerInfo);

        Initialize();
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        Debug.Log("OnRoomPlayerLeft: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        _otherPlayers.Remove(playerInfo);

        Initialize();
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
