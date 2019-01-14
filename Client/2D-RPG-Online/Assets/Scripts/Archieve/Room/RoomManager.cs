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

    [SerializeField]
    private TextMeshProUGUI _txtRoomName;
    [SerializeField]
    private Button _btnStartGame;
    [SerializeField]
    private Button _btnReady;
    [SerializeField]
    private Button _btnNotReady;
    [SerializeField]
    private TMP_InputField _inputFieldRoomID;
    [SerializeField]
    private List<RoomClientSlot> _slotList = new List<RoomClientSlot>();

    private RoomPlayerInfo _leaderPlayerInfo;
    private List<MSSRoom> _roomList = new List<MSSRoom>();

    private void Start() {
        NetworkManager.instance.onGameplayServerConnectionSuccess += OnGameplayServerConnectionSuccess;

        NetworkManager.mss.AddEventListener(MSServerEvent.LobbyRefresh, OnLobbyRefreshed);

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

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomChangeLeader, OnRoomLeaderChanged);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatus, OnPlayerReadyStatusChanged);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatusFailed, OnPlayerReadyStatusChangeFailed);
    }

    public void Initialize() {
        RefreshRoomList();
    }

    public void RefreshRoomList() {
        NetworkManager.mss.GetRoomList();
    }

    public void UpdateUI(RoomPlayerInfo playerInfo) {
        if (IsPlayerExists(playerInfo.Username)) {
            UpdateSlot(playerInfo);
        } else {
            PlaceToNewSlot(playerInfo);
        }

        UpdateActionButtons();
    }

    public void ClearSlots() {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            _slotList[ii].Clear();
        }
    }

    public void CreateRoom() {
        MenuManager.instance.SetInteractionOfCreateRoomButton(false);
        StartCoroutine(ICreateRoom());
    }

    public IEnumerator ICreateRoom() {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.CreatedRoom = new MSSRoom();

        roomData.CreatedRoom.Name = CharacterManager.instance.SelectedCharacter.name + "\'s Room";
        roomData.CreatedRoom.IsPrivate = false;
        roomData.CreatedRoom.MaxUserCount = 2;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    public void ReturnRoom() {
        this.Show();
    }

    public void JoinRoom() {
        MenuManager.instance.SetInteractionOfNormalGameButton(false);
        StartCoroutine(IJoinRoom(_inputFieldRoomID.text));
    }

    public void WatchRoom(string id) {

    }

    public void LeaveRoom() {
        Debug.Log("Attemp to leave room!");
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

    public void SetReadyStatus(RoomClientSlot slot) {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.PlayerReadyStatusInfo.IsReady = !slot.IsReady;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomPlayerReadyStatus, data);
    }

    private void UpdateActionButtons() {
        if (CharacterManager.instance.SelectedCharacter.name == _leaderPlayerInfo.Username) {
            if (IsRoomAvailableToStart()) {
                _btnReady.gameObject.SetActive(false);
                _btnNotReady.gameObject.SetActive(false);
                _btnStartGame.gameObject.SetActive(true);
                _btnStartGame.interactable = true;
            } else {
                _btnReady.gameObject.SetActive(false);
                _btnNotReady.gameObject.SetActive(false);
                _btnStartGame.gameObject.SetActive(true);
                _btnStartGame.interactable = false;
            }
        }
    }

    private bool IsRoomAvailableToStart() {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            if (!_slotList[ii].IsReady || _slotList.Count < 2) {
                return false;
            }
        }
        return true;
    }

    private IEnumerator IJoinRoom(string id) {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        RefreshRoomList();

        yield return new WaitUntil(OnLobbyRefreshed);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.JoinedRoom = new MSSRoom();
        roomData.JoinedRoom.Id = id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    private void SendJoinRoomEvent() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.JoinedRoom = new MSSRoom();
        roomData.JoinedRoom.Id = _roomList[0].Id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    private void PlaceToNewSlot(RoomPlayerInfo playerInfo) {
        GetAvailableSlot(playerInfo.TeamId).UpdateUI(playerInfo);
    }

    private void UpdateSlot(RoomPlayerInfo playerInfo) {
        GetSlot(playerInfo.Username).UpdateUI(playerInfo);
    }

    private void ClearSlot(RoomPlayerInfo playerInfo) {
        GetSlot(playerInfo.Username).Clear();
    }

    private RoomClientSlot GetAvailableSlot(string teamId) {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            if (!_slotList[ii].IsFilledSlot && _slotList[ii].TeamID == teamId) {
                return _slotList[ii];
            }
        }

        return null;
    }

    private RoomClientSlot GetSlot(string username) {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            if (_slotList[ii].IsFilledSlot) {
                if (_slotList[ii].Username == username) {
                    return _slotList[ii];
                }
            }
        }

        return null;
    }

    private bool IsPlayerExists(string username) {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            if (_slotList[ii].IsFilledSlot) {
                if (username == _slotList[ii].Username) {
                    return true;
                }
            }
        }
        return false;
    }

    private void SetTxtRoomName(string name) {
        _txtRoomName.text = name;
    }

    private void SetSlotsTeamIds(Google.Protobuf.Collections.RepeatedField<string> teamIds) {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            if (ii % 2 == 0) {
                _slotList[ii].TeamID = teamIds[0];
            } else {
                _slotList[ii].TeamID = teamIds[1];
            }
        }
    }

    private bool OnLobbyRefreshed() {
        return true;
    }

    private bool OnGameplayServerConnectionSuccess() {
        return true;
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        Debug.Log("OnRoomJoinSuccess: " + data);
        MenuManager.instance.SetInteractionOfNormalGameButton(true);

        SetTxtRoomName(data.RoomData.JoinedRoom.Name);
        SetSlotsTeamIds(data.RoomData.JoinedRoom.Teams);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.CreateTeam(data.RoomData.JoinedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        UpdateUI(playerInfo);

        MenuManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        Debug.Log("OnRoomJoinFailed: " + data);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        Debug.Log("OnRoomGetPlayers: " + data);

        SetTxtRoomName(data.RoomData.JoinedRoom.Name);
        SetSlotsTeamIds(data.RoomData.JoinedRoom.Teams);

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            RoomPlayerInfo playerInfo = data.RoomData.PlayerList[ii];

            TeamManager.instance.AddPlayerToTeam(playerInfo);
            UpdateUI(playerInfo);
        }
    }

    private void OnRoomCreated(ShiftServerData data) {
        Debug.Log("OnRoomCreated: " + data);
        MenuManager.instance.SetInteractionOfCreateRoomButton(true);

        SetTxtRoomName(data.RoomData.CreatedRoom.Name);
        SetSlotsTeamIds(data.RoomData.CreatedRoom.Teams);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        if (playerInfo.IsLeader) {
            _leaderPlayerInfo = playerInfo;
        }

        TeamManager.instance.CreateTeam(data.RoomData.CreatedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        UpdateUI(playerInfo);

        MenuManager.instance.Hide();
        this.Show();
    }

    private void OnRoomCreateFailed(ShiftServerData data) {
        Debug.Log("OnRoomCreateFailed: " + data);
        MenuManager.instance.SetInteractionOfCreateRoomButton(true);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        Debug.Log("OnRoomDeleted: " + data);
    }

    private void OnRoomDeleteFailed(ShiftServerData data) {
        Debug.Log("OnRoomDeleteFailed: " + data);
    }

    private void OnRoomPlayerJoined(ShiftServerData data) {
        Debug.Log("OnRoomPlayerJoined: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.AddPlayerToTeam(playerInfo);
        UpdateUI(playerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        Debug.Log("OnRoomPlayerLeft: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.RemovePlayerFromTeam(playerInfo);
        ClearSlot(playerInfo);
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        Debug.Log("OnRoomLeaveSuccess: " + data);
        MenuManager.instance.SetInteractionOfCreateRoomButton(true);
        MenuManager.instance.SetInteractionOfNormalGameButton(true);

        this.Hide();
        MenuManager.instance.Show();

        TeamManager.instance.ClearTeamList();
        ClearSlots();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        Debug.Log("OnRoomLeaveFailed: " + data);
    }

    private void OnRoomLeaderChanged(ShiftServerData data) {
        Debug.Log("OnRoomLeaderChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        if (playerInfo.IsLeader) {
            _leaderPlayerInfo = playerInfo;
        }

        UpdateUI(playerInfo);
    }

    private void OnPlayerReadyStatusChanged(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        UpdateUI(playerInfo);
    }

    private void OnPlayerReadyStatusChangeFailed(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChangeFailed: " + data);
    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        Debug.Log("OnLobbyRefreshed: " + data);

        _roomList = new List<MSSRoom>();

        for (int ii = 0; ii < data.RoomData.Rooms.Count; ii++) {
            MSSRoom MSSRoom = data.RoomData.Rooms[ii];

            _roomList.Add(data.RoomData.Rooms[ii]);
        }

        OnLobbyRefreshed();
    }

}
