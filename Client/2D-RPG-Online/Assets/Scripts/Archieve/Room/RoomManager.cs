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
    private Button _btnGoToLobby;
    [SerializeField]
    private Button _btnLeaveRoom;
    [SerializeField]
    private TextMeshProUGUI _txtRoomName;
    [SerializeField]
    private List<RoomClientSlot> _slotList = new List<RoomClientSlot>();

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

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomChangeLeader, OnRoomLeaderChanged);

        _btnGoToLobby.onClick.AddListener(GoToLobby);
        _btnLeaveRoom.onClick.AddListener(LeaveRoom);
    }

    public void UpdateUI(RoomPlayerInfo playerInfo) {
        if (IsPlayerExists(playerInfo.Username)) {
            UpdateSlot(playerInfo);
        } else {
            PlaceToNewSlot(playerInfo);
        }
    }

    public void ClearSlots() {
        for (int ii = 0; ii < _slotList.Count; ii++) {
            _slotList[ii].Clear();
        }
    }

    public void CreateRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.CreatedRoom = new MSSRoom();

        roomData.CreatedRoom.Name = System.Guid.NewGuid().ToString().Substring(0, 10);
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
        LobbyManager.instance.RefreshLobby();

        this.Hide();
        LobbyManager.instance.Show();
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

    private void OnRoomJoinSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinSuccess: " + data, Log.Type.Server);

        SetTxtRoomName(data.RoomData.JoinedRoom.Name);
        SetSlotsTeamIds(data.RoomData.JoinedRoom.Teams);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.CreateTeam(data.RoomData.JoinedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        UpdateUI(playerInfo);

        LobbyManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinFailed: " + data, Log.Type.Server);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomGetPlayers: " + data, Log.Type.Server);

        SetTxtRoomName(data.RoomData.JoinedRoom.Name);
        SetSlotsTeamIds(data.RoomData.JoinedRoom.Teams);

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            RoomPlayerInfo playerInfo = data.RoomData.PlayerList[ii];

            TeamManager.instance.AddPlayerToTeam(playerInfo);
            UpdateUI(playerInfo);
        }
    }

    private void OnRoomCreated(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreated: " + data, Log.Type.Server);

        SetTxtRoomName(data.RoomData.CreatedRoom.Name);
        SetSlotsTeamIds(data.RoomData.CreatedRoom.Teams);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.CreateTeam(data.RoomData.CreatedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        UpdateUI(playerInfo);

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

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.AddPlayerToTeam(playerInfo);
        UpdateUI(playerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomPlayerLeft: " + data, Log.Type.Server);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.RemovePlayerFromTeam(playerInfo);
        ClearSlot(playerInfo);
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveSuccess: " + data, Log.Type.Server);

        TeamManager.instance.ClearTeamList();
        ClearSlots();

        GoToLobby();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveFailed: " + data, Log.Type.Server);
    }

    private void OnRoomLeaderChanged(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaderChanged: " + data, Log.Type.Server);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        UpdateUI(playerInfo);
    }

}
