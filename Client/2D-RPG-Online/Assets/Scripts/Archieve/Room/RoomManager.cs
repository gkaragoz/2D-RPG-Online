using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private List<RoomPlayerInfo> _playerList = new List<RoomPlayerInfo>();

    private RoomPlayerInfo _leaderPlayerInfo;

    private void Start() {
        NetworkManager.instance.onGameplayServerConnectionSuccess += OnGameplayServerConnectionSuccess;

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoin, OnRoomJoinSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomJoinFailed, OnRoomJoinFailed);

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

    private IEnumerator IJoinRoom(string id) {
        NetworkManager.instance.ConnectToGameplayServer();

        yield return new WaitUntil(OnGameplayServerConnectionSuccess);

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
        roomData.JoinedRoom.Id = _inputFieldRoomID.text;

        data.RoomData = roomData;

        NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
    }

    private bool IsPlayerExists(RoomPlayerInfo playerInfo) {
        for (int ii = 0; ii < _playerList.Count; ii++) {
            if (_playerList[ii] == playerInfo) {
                return true;
            }
        }
        return false;
    }

    private void SetTxtRoomName(string name) {
        _txtRoomName.text = name;
    }

    private RoomPlayerInfo GetRoomLeader(List<RoomPlayerInfo> players) {
        for (int ii = 0; ii < players.Count; ii++) {
            if (players[ii].IsLeader) {
                return players[ii];
            }
        }
        return null;
    }

    private bool OnGameplayServerConnectionSuccess() {
        return true;
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        Debug.Log("OnRoomJoinSuccess: " + data);
        MenuManager.instance.SetInteractionOfNormalGameButton(true);

        MSSRoom joinedRoom = data.RoomData.JoinedRoom;

        RoomPlayerInfo playerInfo = new RoomPlayerInfo();
        playerInfo = data.RoomData.PlayerInfo;

        MenuManager.instance.Hide();
        this.Show();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        Debug.Log("OnRoomJoinFailed: " + data);

        MenuManager.instance.SetInteractionOfNormalGameButton(true);
    }

    private void OnRoomCreated(ShiftServerData data) {
        Debug.Log("OnRoomCreated: " + data);
        MenuManager.instance.SetInteractionOfCreateRoomButton(true);

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
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        Debug.Log("OnRoomPlayerLeft: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        Debug.Log("OnRoomLeaveSuccess: " + data);
        MenuManager.instance.SetInteractionOfCreateRoomButton(true);
        MenuManager.instance.SetInteractionOfNormalGameButton(true);

        this.Hide();
        MenuManager.instance.Show();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        Debug.Log("OnRoomLeaveFailed: " + data);
    }

    private void OnRoomLeaderChanged(ShiftServerData data) {
        Debug.Log("OnRoomLeaderChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;
    }

    private void OnPlayerReadyStatusChanged(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChanged: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;
    }

    private void OnPlayerReadyStatusChangeFailed(ShiftServerData data) {
        Debug.Log("OnPlayerReadyStatusChangeFailed: " + data);
    }

}
