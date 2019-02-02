using ManaShiftServer.Data.Utils;
using Newtonsoft.Json;
using System;
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

    public List<PlayerController> OtherPlayerControllers { get { return _otherPlayerControllers; } }

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
        if (NetworkManager.mss.IsConnected) {
            var interpolationNow = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            var renderTimestamp = interpolationNow - (1000.0 / serverTickrate);

            for (int ii = 0; ii < OtherPlayerControllers.Count; ii++) {
                PlayerController entity = OtherPlayerControllers[ii];
                NetworkEntity updatedEntityData = entity.NetworkIdentifier;

                // Drop older positions.
                while (entity.NetworkIdentifier.PositionBuffer.Count >= 2 && entity.NetworkIdentifier.PositionBuffer[1].updateTime <= renderTimestamp) {
                    entity.NetworkIdentifier.PositionBuffer = entity.NetworkIdentifier.PositionBuffer.Skip(1).ToList();
                }

                // Interpolate between the two surrounding authoritative positions.
                if (entity.NetworkIdentifier.PositionBuffer.Count >= 2 && entity.NetworkIdentifier.PositionBuffer[0].updateTime <= renderTimestamp && renderTimestamp <= entity.NetworkIdentifier.PositionBuffer[1].updateTime) {
                    Vector3 firstVector = entity.NetworkIdentifier.PositionBuffer[0].vector3;
                    Vector3 secondVector = entity.NetworkIdentifier.PositionBuffer[1].vector3;

                    double t0 = entity.NetworkIdentifier.PositionBuffer[0].updateTime;
                    double t1 = entity.NetworkIdentifier.PositionBuffer[1].updateTime;

                    double interpX = firstVector.x + (secondVector.x - firstVector.x) * (renderTimestamp - t0) / (t1 - t0);
                    double interpZ = firstVector.z + (secondVector.z - firstVector.z) * (renderTimestamp - t0) / (t1 - t0);

                    Vector3 newPosition = new Vector3((float)interpX, 0, (float)interpZ);

                    updatedEntityData.NetworkObject.PositionX = newPosition.x.ToString();
                    updatedEntityData.NetworkObject.PositionY = newPosition.y.ToString();
                    updatedEntityData.NetworkObject.PositionZ = newPosition.z.ToString();
                }

                entity.Render(updatedEntityData);
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
        StartCoroutine(NetworkManager.instance.ConnectToGameplayServer((bool success) => {
            if (success) {
                ShiftServerData data = new ShiftServerData();

                RoomData roomData = new RoomData();
                roomData.Room = new MSSRoom();

                roomData.Room.Name = CharacterManager.instance.SelectedCharacter.name + "\'s Room";
                roomData.Room.IsPrivate = false;
                roomData.Room.MaxUserCount = 1000;

                data.RoomData = roomData;

                NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
            }
        }));
    }

    public void JoinRoom() {
        StartCoroutine(NetworkManager.instance.ConnectToGameplayServer((bool success) => {
            if (success) {
                ShiftServerData data = new ShiftServerData();

                RoomData roomData = new RoomData();
                roomData.Room = new MSSRoom();
                roomData.Room.Id = "123";

                data.RoomData = roomData;

                NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
            }
        }));
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
        GameObject player = Instantiate(_playerPrefab, new Vector3(playerInfo.NetworkObject.PositionX.ToFloat(), playerInfo.NetworkObject.PositionY.ToFloat(), playerInfo.NetworkObject.PositionZ.ToFloat()), Quaternion.identity);

        _myPlayerController = player.GetComponent<PlayerController>();
        _myPlayerController.Initialize(playerInfo.NetworkObject);
    }


    private void CreatePlayer(RoomPlayerInfo playerInfo) {
        NetworkIdentifier tempNetworkObject = playerInfo.NetworkObject;

        playerInfo.NetworkObject = new NetworkIdentifier();
        playerInfo.NetworkObject = tempNetworkObject;

        PlayerController playerController = Instantiate(_playerPrefab, new Vector3(playerInfo.NetworkObject.PositionX.ToFloat(), playerInfo.NetworkObject.PositionY.ToFloat(), playerInfo.NetworkObject.PositionZ.ToFloat()), Quaternion.identity).GetComponent<PlayerController>();
        playerController.Initialize(playerInfo.NetworkObject);

        OtherPlayerControllers.Add(playerController);
    }

    private void OnRoomUpdated(ShiftServerData data) {
        serverTickrate = data.SvTickRate;

        Debug.Log(data);

        for (int ii = 0; ii < data.StateUpdate.NetworkObjects.Count; ii++) {
            NetworkIdentifier updatedNetworkObject = data.StateUpdate.NetworkObjects[ii];
            if (updatedNetworkObject.Id == _myPlayerController.NetworkIdentifier.Oid) {

                //_myPlayerController.transform.position = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);

                //My Reconciliation
                if (NetworkManager.instance.Reconciliaton) {
                    for (int jj = 0; jj < _myPlayerController.NetworkIdentifier.PlayerInputs.Count; jj++) {
                        if (_myPlayerController.NetworkIdentifier.GetSequenceID(jj) <= updatedNetworkObject.LastProcessedInputID) {
                            _myPlayerController.NetworkIdentifier.RemoveRange(jj, 1);
                        } else {
                            // re apply
                        }
                    }
                } else {
                    _myPlayerController.NetworkIdentifier.ClearPlayerInputs();
                }
            }

            //Other Entity's Movement
            for (int jj = 0; jj < OtherPlayerControllers.Count; jj++) {
                if (OtherPlayerControllers[jj].NetworkIdentifier.Oid == updatedNetworkObject.Id) {
                    if (Utils.IsValid(updatedNetworkObject.PositionX, updatedNetworkObject.PositionY, updatedNetworkObject.PositionZ)) {
                        Vector3 updatedPosition = new Vector3(updatedNetworkObject.PositionX.ToFloat(), updatedNetworkObject.PositionY.ToFloat(), updatedNetworkObject.PositionZ.ToFloat());

                        if (OtherPlayerControllers[jj].NetworkIdentifier.LastProcessedInputSequenceID <= updatedNetworkObject.LastProcessedInputID) {
                            DateTime updateTime = DateTime.UtcNow;
                            var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                            OtherPlayerControllers[jj].NetworkIdentifier.AddPositionToBuffer(now, updatedPosition, updatedNetworkObject.LastProcessedInputID);
                        }

                        OtherPlayerControllers[jj].NetworkIdentifier.LastProcessedInputSequenceID = updatedNetworkObject.LastProcessedInputID;
                    }
                }
            }
        }
    }

    private void OnPlayerCreated(ShiftServerData data) {
        Debug.Log("OnPlayerCreated: " + data);
        
        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        if (playerInfo.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
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

        //for (int ii = 0; ii < OtherPlayerControllers.Count; ii++) {
        //    if (playerInfo.CurrentGObject.Oid == OtherPlayerControllers[ii].Oid) {
        //        OtherPlayerControllers.Remove(OtherPlayerControllers[ii]);
        //        OtherPlayerControllers[ii].Destroy();
        //        break;
        //    }
        //}
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
