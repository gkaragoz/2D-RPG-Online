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

    [SerializeField]
    private Button _btnCreateRoom;
    [SerializeField]
    private Button _btnRefreshLobby;

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

        RefreshLobby();

        _btnCreateRoom.onClick.AddListener(CreateRoom);
        _btnRefreshLobby.onClick.AddListener(delegate {
            RefreshLobby();
        });

        onReturnButtonClicked = RoomManager.instance.ReturnRoom;
        onJoinButtonClicked = RoomManager.instance.JoinRoom;
        onWatchButtonClicked = RoomManager.instance.WatchRoom;
    }

    public void UpdateUI() {
        SetBtnCreateRoomInteraction();
        SetRowButtonsInteractions();
    }

    public void CreateRoom() {
        RoomManager.instance.CreateRoom();
    }

    public void RefreshLobby() {
        NetworkManager.mss.GetRoomList();

        UpdateUI();
    }

    public void CreateLobbyRow(int rowNumber, MSSRoom MSSRoom) {
        LobbyRow lobbyRow = Instantiate(_lobbyRowPrefab, _gridContainer);

        lobbyRow.UpdateUI(rowNumber, MSSRoom);
        lobbyRow.SetJoinRoomButtonOnClickAction(onJoinButtonClicked);
        lobbyRow.SetWatchRoomButtonOnClickAction(onWatchButtonClicked);
        lobbyRow.SetReturnRoomButtonOnClickAction(onReturnButtonClicked);

        _lobbyRowsList.Add(lobbyRow);
    }

    public void UpdateLobbyRow(int rowNumber, MSSRoom MSSRoom) {
        string roomID = MSSRoom.Id;

        _lobbyRowsList.Find(row => row.RoomID == roomID).UpdateUI(rowNumber, MSSRoom);
    }

    public void RemoveLobbyRow(MSSRoom MSSRoom) {
        string roomID = MSSRoom.Id;

        LobbyRow lobbyRow = _lobbyRowsList.Find(row => row.RoomID == roomID);
        _lobbyRowsList.Remove(lobbyRow);
        lobbyRow.Destroy();
    }

    private void SetRowButtonsInteractions() {
        for (int ii = 0; ii < _lobbyRowsList.Count; ii++) {
            _lobbyRowsList[ii].SetJoinRoomButtonInteractions();
            _lobbyRowsList[ii].SetWatchRoomButtonInteractions();
        }
    }

    private void SetBtnCreateRoomInteraction() {
        _btnCreateRoom.interactable = !NetworkManager.mss.HasPlayerRoom;
    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        LogManager.instance.AddLog("OnLobbyRefreshed: " + data, Log.Type.Server);

        for (int ii = 0; ii < data.RoomData.Rooms.Count; ii++) {
            MSSRoom MSSRoom = data.RoomData.Rooms[ii];

            if (IsLobbyRowExists(MSSRoom.Id)) {
                UpdateLobbyRow(1, MSSRoom);
            } else {
                CreateLobbyRow(1, MSSRoom);
            }
        }
    }

    private bool IsLobbyRowExists(string roomID) {
        for (int ii = 0; ii < _lobbyRowsList.Count; ii++) {
            if (roomID == _lobbyRowsList[ii].RoomID) {
                return true;
            }
        }
        return false;
    }
    
}
