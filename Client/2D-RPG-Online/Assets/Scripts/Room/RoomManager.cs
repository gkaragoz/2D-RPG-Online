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

    [Serializable]
    public class UISettings {
        public Button btnGoToLobby;
        public Button btnLeaveRoom;
        public TextMeshProUGUI txtRoomName;
        public List<RoomClientSlot> _slotList = new List<RoomClientSlot>();

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

        public void SetTxtRoomName(string name) {
            txtRoomName.text = name;
        }

        private void PlaceToNewSlot(RoomPlayerInfo playerInfo) {
            GetAvailableSlot().Initialize(playerInfo);
        }

        private void UpdateSlot(RoomPlayerInfo playerInfo) {
            GetSlot(playerInfo.Username).Initialize(playerInfo);
        }

        private void ClearSlot(RoomPlayerInfo playerInfo) {
            GetSlot(playerInfo.Username).Clear();
        }

        private RoomClientSlot GetAvailableSlot() {
            for (int ii = 0; ii < _slotList.Count; ii++) {
                if (!_slotList[ii].IsFilledSlot) {
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
    }

    [SerializeField]
    private UISettings _UISettings;

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
    
    private void OnRoomJoinSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinSuccess: " + data, Log.Type.Server);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo.Username = NetworkManager.mss.AccountData.Username;
        playerInfo.TeamId = data.RoomData.PlayerInfo.TeamId;

        TeamManager.instance.CreateTeam(data.RoomData.JoinedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        _UISettings.UpdateUI(playerInfo);

        LobbyManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomJoinFailed: " + data, Log.Type.Server);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomGetPlayers: " + data, Log.Type.Server);

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            RoomPlayerInfo playerInfo = data.RoomData.PlayerList[ii];

            TeamManager.instance.AddPlayerToTeam(playerInfo);
            _UISettings.UpdateUI(playerInfo);
        }
    }

    private void OnRoomCreated(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomCreated: " + data, Log.Type.Server);

        LobbyManager.instance.CreateLobbyRow(data.RoomData.CreatedRoom);
        _UISettings.SetTxtRoomName(data.RoomData.CreatedRoom.Name);

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo.Username = NetworkManager.mss.AccountData.Username;
        playerInfo.TeamId = data.RoomData.PlayerInfo.TeamId;

        TeamManager.instance.CreateTeam(data.RoomData.CreatedRoom.Teams);
        TeamManager.instance.AddPlayerToTeam(playerInfo);

        _UISettings.UpdateUI(playerInfo);

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
        _UISettings.UpdateUI(playerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomPlayerLeft: " + data, Log.Type.Server);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        TeamManager.instance.RemovePlayerFromTeam(playerInfo);
        _UISettings.UpdateUI(playerInfo);
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveSuccess: " + data, Log.Type.Server);

        TeamManager.instance.ClearTeamList();
        _UISettings.ClearSlots();

        GoToLobby();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        LogManager.instance.AddLog("OnRoomLeaveFailed: " + data, Log.Type.Server);
    }

}
