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

    [SerializeField]
    private Button _btnLeaveRoom;
    [SerializeField]
    private TextMeshProUGUI _txtRoomName;
    [SerializeField]
    private List<RoomClientSlot> _slotList = new List<RoomClientSlot>();

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

        _btnLeaveRoom.onClick.AddListener(LeaveRoom);
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

        roomData.CreatedRoom.Name = System.Guid.NewGuid().ToString().Substring(0, 10);
        roomData.CreatedRoom.IsPrivate = false;
        roomData.CreatedRoom.MaxUserCount = 5;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
    }

    public void ReturnRoom() {
        this.Show();
    }

    public void JoinRoom(string id) {
        MenuManager.instance.SetInteractionOfNormalGameButton(false);
        StartCoroutine(IJoinRoom(id));
    }

    public IEnumerator IJoinRoom(string id) {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.JoinedRoom = new MSSRoom();
        roomData.JoinedRoom.Id = id;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    public void JoinFirstRoom() {
        MenuManager.instance.SetInteractionOfNormalGameButton(false);
        StartCoroutine(IJoinFirstRoom());
    }

    public IEnumerator IJoinFirstRoom() {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.JoinedRoom = new MSSRoom();
        roomData.JoinedRoom.Id = _roomList[0].Id;

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

        TeamManager.instance.ClearTeamList();
        ClearSlots();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        Debug.Log("OnRoomLeaveFailed: " + data);
    }

    private void OnRoomLeaderChanged(ShiftServerData data) {
        Debug.Log("OnRoomLeaderChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        UpdateUI(playerInfo);
    }

    private void OnLobbyRefreshed(ShiftServerData data) {
        Debug.Log("OnLobbyRefreshed: " + data);

        _roomList = new List<MSSRoom>();

        for (int ii = 0; ii < data.RoomData.Rooms.Count; ii++) {
            MSSRoom MSSRoom = data.RoomData.Rooms[ii];

            _roomList.Add(data.RoomData.Rooms[ii]);
        }
    }

}
