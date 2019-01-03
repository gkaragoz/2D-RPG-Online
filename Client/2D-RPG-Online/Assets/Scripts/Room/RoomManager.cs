using System.Collections;
using System.Collections.Generic;
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
    }

    [SerializeField]
    private UISettings _UISettings;

    public void Initialize() {
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoin, OnRoomJoinSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoinFailed, OnRoomJoinFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreate, OnRoomCreated);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreateFailed, OnRoomCreateFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDelete, OnRoomDeleted);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDeleteFailed, OnRoomDeleteFailed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomGetInfo, OnRoomGetInfo);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeave, OnRoomLeaveSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomLeaveFailed, OnRoomLeaveFailed);

        _UISettings.btnGoToLobby.onClick.AddListener(GoToLobby);
        _UISettings.btnLeaveRoom.onClick.AddListener(LeaveRoom);
    }

    public void CreateRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.CreatedRoom = new ServerRoom();

        roomData.CreatedRoom.Name = "Odanın adını Feriha koydum.";
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
        roomData.JoinedRoom = new ServerRoom();
        roomData.JoinedRoom.Id = id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    public void WatchRoom(string id) {

    }

    public void LeaveRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.LeavedRoom = new ServerRoom();

        NetworkManager.mss.SendMessage(MSServerEvent.RoomLeave);
    }

    public void DeleteRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.DeletedRoom = new ServerRoom();
        roomData.DeletedRoom.Id = NetworkManager.mss.JoinedRoom.Id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomDelete, data);
    }

    public void GoToLobby() {
        LobbyManager.instance.RefreshLobby(false);

        this.Hide();
        LobbyManager.instance.Show();
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinSuccess: " + data, Log.Type.Server);

        LobbyManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinFailed: " + data, Log.Type.Server);
    }

    private void OnRoomCreated(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreated: " + data, Log.Type.Server);

        ServerRoom serverRoom = data.RoomData.CreatedRoom;

        LobbyManager.instance.CreateLobbyRow(serverRoom);

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

    private void OnRoomGetInfo(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomGetInfo: " + data, Log.Type.Server);
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveSuccess: " + data, Log.Type.Server);

        RoomManager.instance.Hide();
        this.Show();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveFailed: " + data, Log.Type.Server);
    }

}
