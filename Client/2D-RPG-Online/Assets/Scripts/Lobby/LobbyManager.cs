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

    public delegate void JoinDelegate(string id);
    public JoinDelegate onJoinButtonClicked;

    public delegate void WatchDelegate(string id);
    public WatchDelegate onWatchButtonClicked;

    public delegate void ReturnDelegate(string id);
    public ReturnDelegate onReturnButtonClicked;

    [System.Serializable]
    public class UISettings {
        public Button btnCreateRoom;
        public Button btnRefreshLobby;

        public void ActivateCreateRoomButton() {
            btnCreateRoom.interactable = true;
        }

        public void DeactivateCreateRoomButton() {
            btnCreateRoom.interactable = false;
        }
    }

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

        RefreshLobby(true);

        _UISettings.btnCreateRoom.onClick.AddListener(CreateRoom);
        _UISettings.btnRefreshLobby.onClick.AddListener(delegate {
            RefreshLobby(true);
        });

        onReturnButtonClicked = RoomManager.instance.ReturnRoom;
        onJoinButtonClicked = RoomManager.instance.JoinRoom;
        onWatchButtonClicked = RoomManager.instance.WatchRoom;
    }

    public void CreateRoom() {
        RoomManager.instance.CreateRoom();
    }

    public void RefreshLobby(bool fromNetwork) {
        if (NetworkManager.mss.JoinedRoom != null) {
            _UISettings.DeactivateCreateRoomButton();
        } else {
            _UISettings.ActivateCreateRoomButton();
        }

        if (fromNetwork) {
            NetworkManager.mss.GetRoomList();
        } else {
            for (int ii = 0; ii < _lobbyRowsList.Count; ii++) {
                _lobbyRowsList[ii].UpdateUI(NetworkManager.mss.JoinedRoom.Id);
            }
        }
    }

    public void CreateLobbyRow(ServerRoom serverRoom) {
        LobbyRow lobbyRow = Instantiate(_lobbyRowPrefab, _gridContainer);

        lobbyRow.Initialize(1, serverRoom);
        lobbyRow.SetJoinButtonOnClickAction(onJoinButtonClicked);
        lobbyRow.SetWatchButtonOnClickAction(onWatchButtonClicked);

        _lobbyRowsList.Add(lobbyRow);
    }

    public void UpdateLobbyRow(LobbyRow lobbyRow, ServerRoom serverRoom) {
        lobbyRow.Initialize(1, serverRoom);
    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        LogManager.instance.AddLog("OnLobbyRefreshed: " + data, Log.Type.Server);

        for (int ii = 0; ii < data.RoomData.Rooms.Count; ii++) {
            ServerRoom serverRoom = data.RoomData.Rooms[ii];

            if (IsLobbyRowExists(serverRoom.Id)) {
                UpdateLobbyRow(_lobbyRowsList[ii], serverRoom);
            } else {
                CreateLobbyRow(serverRoom);
            }
        }
    }

    private bool IsLobbyRowExists(string roomID) {
        for (int ii = 0; ii < _lobbyRowsList.Count; ii++) {
            if (roomID == _lobbyRowsList[ii].GetRoomID) {
                return true;
            }
        }
        return false;
    }
    
}
