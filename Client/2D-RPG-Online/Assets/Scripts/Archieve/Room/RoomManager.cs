using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    private void FixedUpdate() {
        var interpolationNow = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        var renderTimestamp = interpolationNow - (1000.0 / serverTickrate);

        for (int ii = 0; ii < _otherPlayerControllers.Count; ii++) {
            PlayerController entity = _otherPlayerControllers[ii];

            // Drop older positions.
            while (entity.PositionBuffer.Count >= 2 && entity.PositionBuffer[1].updateTime <= renderTimestamp) {
                entity.PositionBuffer = entity.PositionBuffer.Skip(1).ToList();
            }

            // Interpolate between the two surrounding authoritative positions.
            if (entity.PositionBuffer.Count >= 2 && entity.PositionBuffer[0].updateTime <= renderTimestamp && renderTimestamp <= entity.PositionBuffer[1].updateTime) {
                Vector3 firstVector = entity.PositionBuffer[0].vector3;
                Vector3 secondVector = entity.PositionBuffer[1].vector3;

                double t0 = entity.PositionBuffer[0].updateTime;
                double t1 = entity.PositionBuffer[1].updateTime;

                double interpX = firstVector.x + (secondVector.x - firstVector.x) * (renderTimestamp - t0) / (t1 - t0);
                double interpZ = firstVector.z + (secondVector.z - firstVector.z) * (renderTimestamp - t0) / (t1 - t0);

                //entity.transform.position = Vector3.Lerp(firstVector, secondVector, (float)(renderTimestamp - t0) / (float)(t1 - t0));

                Vector3 newPosition = new Vector3((float)interpX, 0, (float)interpZ);

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

    private void OnRoomUpdated(ShiftServerData data) {
        serverTickrate = data.SvTickRate;

        //Debug.Log(data);

        for (int ii = 0; ii < data.GoUpdatePacket.PlayerList.Count; ii++) {
            PlayerObject updatedPlayerObject = data.GoUpdatePacket.PlayerList[ii];
            if (updatedPlayerObject.Oid == _myPlayerController.Oid) {

                //_myPlayerController.transform.position = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);

                //My Reconciliation
                if (NetworkManager.instance.Reconciliaton) {
                    for (int jj = 0; jj < _myPlayerController.PlayerInputs.Count; jj++) {
                        if (_myPlayerController.GetSequenceID(jj) <= updatedPlayerObject.LastProcessedSequenceID) {
                            _myPlayerController.RemoveRange(jj, 1);
                        } else {
                            // re apply
                        }
                    }
                } else {
                    _myPlayerController.ClearPlayerInputs();
                }
            }

            //Other Entity's Movement
            for (int jj = 0; jj < _otherPlayerControllers.Count; jj++) {
                if (_otherPlayerControllers[jj].Oid == updatedPlayerObject.Oid) {
                    Vector3 updatedPosition = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);

                    if (_otherPlayerControllers[jj].LastProcessedInputSequenceID <= updatedPlayerObject.LastProcessedSequenceID) {
                        DateTime updateTime = DateTime.UtcNow;
                        var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        _otherPlayerControllers[jj].AddPositionToBuffer(now, updatedPosition, updatedPlayerObject.LastProcessedSequenceID);
                    }
                    _otherPlayerControllers[jj].LastProcessedInputSequenceID = updatedPlayerObject.LastProcessedSequenceID;
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
