using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [System.Serializable]
    public class UISettings {
        public Button btnGoToLobby;
        public Button btnLeaveRoom;
        public TextMeshProUGUI txtRoomName;

        public void SetTxtRoomName(string name) {
            txtRoomName.text = name;
        }
    }

    [SerializeField]
    private RoomClientSlot _roomLeftClientSlotPrefab;
    [SerializeField]
    private RoomClientSlot _roomRightClientSlotPrefab;
    [SerializeField]
    private RectTransform _leftSlotParent;
    [SerializeField]
    private RectTransform _rightSlotParent;
    [SerializeField]
    private UISettings _UISettings;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private List<RoomClientSlot> _roomLeftClientSlotsList = new List<RoomClientSlot>();
    private List<RoomClientSlot> _roomRightClientSlotsList = new List<RoomClientSlot>();

    public void Initialize() {
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoin, OnRoomJoinSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoinFailed, OnRoomJoinFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomGetPlayers, OnRoomGetPlayers);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreate, OnRoomCreated);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreateFailed, OnRoomCreateFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDelete, OnRoomDeleted);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDeleteFailed, OnRoomDeleteFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerJoined, OnRoomPlayerJoined);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerLeft, OnRoomPlayerLeft);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeave, OnRoomLeaveSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeaveFailed, OnRoomLeaveFailed);

        _UISettings.btnGoToLobby.onClick.AddListener(GoToLobby);
        _UISettings.btnLeaveRoom.onClick.AddListener(LeaveRoom);
    }

    public void CreateRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.CreatedRoom = new MSSRoom();

        roomData.CreatedRoom.Name = Guid.NewGuid().ToString().Substring(0, 10);
        roomData.CreatedRoom.IsPrivate = false;
        roomData.CreatedRoom.MaxUserCount = 5;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    public void ReturnRoom(string id) {
        LobbyManager.instance.Hide();
        this.Show();
    }

    public void JoinRoom(string id) {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.JoinedRoom = new MSSRoom();
        roomData.JoinedRoom.Id = id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    public void WatchRoom(string id) {

    }

    public void LeaveRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.LeavedRoom = new MSSRoom();

        NetworkManager.mss.SendMessage(MSServerEvent.RoomLeave);
    }

    public void DeleteRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.DeletedRoom = new MSSRoom();
        roomData.DeletedRoom.Id = NetworkManager.mss.JoinedRoom.Id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomDelete, data);
    }

    public void GoToLobby() {
        LobbyManager.instance.RefreshLobby(false);

        this.Hide();
        LobbyManager.instance.Show();
    }

    private void CreateRoomClientSlot(RoomPlayerInfo roomPlayerInfo) {
        RoomClientSlot roomClientSlot = Instantiate(_roomLeftClientSlotPrefab, _leftSlotParent);
        roomClientSlot.Initialize(roomPlayerInfo);

        _roomLeftClientSlotsList.Add(roomClientSlot);
    }

    private void UpdateRoomClientSlot(RoomPlayerInfo roomPlayerInfo) {
        string username = roomPlayerInfo.Username;

        _roomLeftClientSlotsList.Find(slot => slot.Username == username).Initialize(roomPlayerInfo);
    }

    private void RemoveRoomClientSlot(RoomPlayerInfo roomPlayerInfo) {
        string username = roomPlayerInfo.Username;

        RoomClientSlot roomClientSlot = _roomLeftClientSlotsList.Find(slot => slot.Username == username);
        _roomLeftClientSlotsList.Remove(roomClientSlot);
        roomClientSlot.Destroy();
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinSuccess: " + data, Log.Type.Server);

        LobbyManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinFailed: " + data, Log.Type.Server);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomGetPlayers: " + data, Log.Type.Server);

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            RoomPlayerInfo roomPlayerInfo = data.RoomData.PlayerList[ii];

            if (IsRoomPlayerExists(roomPlayerInfo.Username)) {
                CreateRoomClientSlot(roomPlayerInfo);
            } else {
                UpdateRoomClientSlot(roomPlayerInfo);
            }
        }
    }

    private void OnRoomCreated(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreated: " + data, Log.Type.Server);

        MSSRoom MSSRoom = data.RoomData.CreatedRoom;

        LobbyManager.instance.CreateLobbyRow(MSSRoom);
        _UISettings.SetTxtRoomName(MSSRoom.Name);

        RoomPlayerInfo roomPlayerInfo = new RoomPlayerInfo();
        roomPlayerInfo.Username = "USERNAMEI MSS VERCEK";

        CreateRoomClientSlot(roomPlayerInfo);

        LobbyManager.instance.Hide();
        this.Show();
    }

    private void OnRoomCreateFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreateFailed: " + data, Log.Type.Server);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomDeleted: " + data, Log.Type.Server);
    }

    private void OnRoomDeleteFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomDeleteFailed: " + data, Log.Type.Server);
    }

    private void OnRoomPlayerJoined(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomPlayerJoined: " + data, Log.Type.Server);

        CreateRoomClientSlot(data.RoomData.PlayerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomPlayerLeft: " + data, Log.Type.Server);

        RemoveRoomClientSlot(data.RoomData.PlayerInfo);
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveSuccess: " + data, Log.Type.Server);

        GoToLobby();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveFailed: " + data, Log.Type.Server);
    }

    private bool IsRoomPlayerExists(string username) {
        for (int ii = 0; ii < _roomLeftClientSlotsList.Count; ii++) {
            if (username == _roomLeftClientSlotsList[ii].Username) {
                return true;
            }
        }
        return false;
    }

}
