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
    private LobbyRow _lobbyRowPrefab;
    [SerializeField]
    private RectTransform _gridContainer;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private List<LobbyRow> _lobbyRowsList = new List<LobbyRow>();

    public void Initialize() {
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

    public void CreateRoom() {
        ShiftServerData data = new ShiftServerData();
        
        RoomData roomData = new RoomData();
        roomData.CreatedRoom.Name = "Odanın adını Feriha koydum.";
        roomData.CreatedRoom.CreatedTime = DateTime.Now.ToString();
        roomData.CreatedRoom.IsPrivate = false;
        roomData.CreatedRoom.MaxUserCount = 5;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        LogManager.instance.AddLog("OnLobbyRefreshed: " + data, Log.Type.Server);

        for (int ii = 0; ii < data.RoomData.Rooms.Count; ii++) {
            string roomID = data.RoomData.Rooms[ii].Id;
            string roomName = data.RoomData.Rooms[ii].Name;
            int currentUsersCount = data.RoomData.Rooms[ii].CurrentUserCount == 0 ? 0 : data.RoomData.Rooms[ii].CurrentUserCount;
            int maxUsersCount = data.RoomData.Rooms[ii].MaxUserCount;
            bool isPrivate = data.RoomData.Rooms[ii].IsPrivate;

            for (int jj = 0; jj < _lobbyRowsList.Count; jj++) {
                if (roomID == _lobbyRowsList[ii].RoomID) {
                    UpdateLobbyRow(
                        _lobbyRowsList[ii],
                        roomID,
                        roomName,
                        currentUsersCount,
                        maxUsersCount,
                        isPrivate
                        );
                }
            }

            CreateLobbyRow(
                roomID, 
                roomName, 
                currentUsersCount, 
                maxUsersCount, 
                isPrivate
                );
        }
    }

    private void RefreshLobby() {
        NetworkManager.mss.GetRoomList();
    }

    private void CreateLobbyRow(string roomID, string roomName, int currentUsersCount, int maxUsersCount, bool isPrivate) {
        LobbyRow lobbyRow = Instantiate(_lobbyRowPrefab, _gridContainer);

        //lobbyRow.RoomName = GetRowNumber();
        lobbyRow.RoomID = roomID;
        lobbyRow.RoomName = roomName;
        lobbyRow.CurrentUsersCount = currentUsersCount;
        lobbyRow.MaxUsersCount = maxUsersCount;
        lobbyRow.IsPrivate = isPrivate;

        _lobbyRowsList.Add(lobbyRow);
    }

    private void UpdateLobbyRow(LobbyRow lobbyRow, string roomID, string roomName, int currentUsersCount, int maxUsersCount, bool isPrivate) {
        lobbyRow.RoomID = roomID;
        lobbyRow.RoomName = roomName;
        lobbyRow.CurrentUsersCount = currentUsersCount;
        lobbyRow.MaxUsersCount = maxUsersCount;
        lobbyRow.IsPrivate = isPrivate;
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
