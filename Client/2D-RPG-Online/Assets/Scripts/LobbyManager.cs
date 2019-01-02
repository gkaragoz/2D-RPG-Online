using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Menu {

    #region Singleton

    public static LobbyManager instance;

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
        public Button btnCreateRoom;
        public Button btnRefreshLobby;
    }

    [Header("Initialization")]
    [SerializeField]
    private UISettings _UISettings;
    [SerializeField]
    private LobbyGridRow _lobbyGridRowPrefab;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private List<LobbyGridRow> _lobbyRows = new List<LobbyGridRow>();

    private void Start() {
        NetworkManager.mss.AddEventListener(MSServerEvent.LobbyRefresh, OnLobbyRefreshed);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoin, OnRoomJoinSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoinFailed, OnRoomJoinFailed);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomCreate, OnRoomCreated);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomDelete, OnRoomDeleted);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomGetInfo, OnRoomGetInfo);

        RefreshLobby();

        _UISettings.btnCreateRoom.onClick.AddListener(CreateRoom);
        _UISettings.btnRefreshLobby.onClick.AddListener(RefreshLobby);
    }

    public void InitializeLobbyGrid() {

    }

    public void RefreshLobby() {
        NetworkManager.mss.SendMessage(MSServerEvent.LobbyRefresh);
    }

    public void GetRoomList() {

    }

    public void CreateLobbyRow() {

    }

    public void CreateRoom() {

    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        InitializeLobbyGrid();
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinSuccess: " + data, Log.Type.Server);
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinFailed: " + data, Log.Type.Server);
    }

    private void OnRoomCreated(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreated: " + data, Log.Type.Server);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomDeleted: " + data, Log.Type.Server);
    }

    private void OnRoomGetInfo(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomGetInfo: " + data, Log.Type.Server);
    }

}
