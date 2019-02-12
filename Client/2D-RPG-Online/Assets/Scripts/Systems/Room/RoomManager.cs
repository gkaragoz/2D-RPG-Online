﻿using ManaShiftServer.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public Action onRoomCreated;
    public Action onRoomJoined;
    public Action onRoomLeft;

    public int OtherPlayersCount { get { return _otherPlayerControllers.Count; } }

    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private Button _btnStartGame;
    [SerializeField]
    private Button _btnReady;
    [SerializeField]
    private Button _btnNotReady;
    [SerializeField]
    private Button _btnLeave;

    private Dictionary<int, PlayerController> _otherPlayerControllers = new Dictionary<int, PlayerController>();
    private List<RoomPlayerInfo> _otherPlayerRoomInfos = new List<RoomPlayerInfo>();

    private PlayerController _myPlayerController;
    private RoomPlayerInfo _myRoomPlayerInfo;

    private bool _hasInitialized = false;

    private void Start() {
        NetworkManager.mss.AddEventListener(MSPlayerEvent.UpdatePlayer, OnPlayerUpdated);
        NetworkManager.mss.AddEventListener(MSPlayerEvent.Attack, OnPlayerAttackReceived);
        NetworkManager.mss.AddEventListener(MSPlayerEvent.Spell, OnPlayerSpellReceived);

        NetworkManager.mss.AddEventListener(MSPlayerEvent.CreatePlayer, OnPlayerCreated);

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

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomGetPlayers, OnRoomGetPlayers);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomChangeLeader, OnRoomLeaderChanged);

        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatus, OnPlayerReadyStatusChanged);
        NetworkManager.mss.AddEventListener(MSServerEvent.RoomPlayerReadyStatusFailed, OnPlayerReadyStatusChangeFailed);

        SceneController.instance.onSceneLoaded = OnSceneLoaded;
    }

    private void Update() {
        if (NetworkManager.mss == null) {
            return;
        }

        if (NetworkManager.mss.IsConnected && NetworkManager.mss.HasPlayerRoom) {
            double interpolationNow = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            double renderTimestamp = interpolationNow - (1000.0 / NetworkManager.mss.JoinedRoom.ServerTickRate);

            for (int ii = 0; ii < _otherPlayerControllers.Count; ii++) {
                PlayerController entity = _otherPlayerControllers.ElementAt(ii).Value;

                ProcessInterpolation(renderTimestamp, entity);
            }
        }
    }

    public void OnSceneLoaded() {
        if (SceneController.instance.GetActiveScene().name == "Gameplay") {
            CreateMyPlayer();
            for (int ii = 0; ii < _otherPlayerRoomInfos.Count; ii++) {
                CreatePlayer(_otherPlayerRoomInfos[ii]);
            }

            _hasInitialized = true;
        } else {
            _hasInitialized = false;
        }
    }

    public PlayerController GetPlayerByIndex(int index) {
        if (_otherPlayerControllers != null) {
            if (index <= _otherPlayerControllers.Count) {
                return _otherPlayerControllers.ElementAt(index).Value;
            }
        }

        return null;
    }

    public void CreateRoom() {
        StartCoroutine(NetworkManager.instance.ConnectToGameplayServer((bool success) => {
            if (success) {
                ShiftServerData data = new ShiftServerData();

                RoomData roomData = new RoomData();
                roomData.Room = new MSSRoom();

                roomData.Room.Name = CharacterManager.instance.SelectedCharacter.name + "\'s Room";
                roomData.Room.IsPrivate = false;
                roomData.Room.MaxUserCount = 1000;

                data.RoomData = roomData;

                NetworkManager.mss.SendMessage(MSServerEvent.RoomCreate, data);
            }
        }));
    }

    public void JoinRoom() {
        StartCoroutine(NetworkManager.instance.ConnectToGameplayServer((bool success) => {
            if (success) {
                ShiftServerData data = new ShiftServerData();

                RoomData roomData = new RoomData();
                roomData.Room = new MSSRoom();
                roomData.Room.Id = "123";

                data.RoomData = roomData;

                NetworkManager.mss.SendMessage(MSServerEvent.RoomJoin, data);
            }
        }));
    }

    public void LeaveRoom() {
        Debug.Log("Attemp to leave room!");
        NetworkManager.mss.SendMessage(MSServerEvent.RoomLeave);
    }

    public void DeleteRoom() {
        ShiftServerData data = new ShiftServerData();

        RoomData roomData = new RoomData();
        roomData.Room = new MSSRoom();
        roomData.Room.Id = NetworkManager.mss.JoinedRoom.Id;

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

    private void CreateMyPlayer() {
        GameObject player = Instantiate(_playerPrefab, new Vector3(_myRoomPlayerInfo.NetworkObject.PositionX.ToFloat(), _myRoomPlayerInfo.NetworkObject.PositionY.ToFloat(), _myRoomPlayerInfo.NetworkObject.PositionZ.ToFloat()), Quaternion.identity);

        _myPlayerController = player.GetComponent<PlayerController>();
        _myPlayerController.Initialize(_myRoomPlayerInfo.NetworkObject);
    }


    private void CreatePlayer(RoomPlayerInfo playerInfo) {
        NetworkIdentifier tempNetworkObject = playerInfo.NetworkObject;

        playerInfo.NetworkObject = new NetworkIdentifier();
        playerInfo.NetworkObject = tempNetworkObject;

        PlayerController playerController = Instantiate(_playerPrefab, new Vector3(playerInfo.NetworkObject.PositionX.ToFloat(), playerInfo.NetworkObject.PositionY.ToFloat(), playerInfo.NetworkObject.PositionZ.ToFloat()), Quaternion.identity).GetComponent<PlayerController>();
        playerController.Initialize(playerInfo.NetworkObject);

        _otherPlayerControllers.Add(playerInfo.NetworkObject.Id, playerController);
    }

    private void OnPlayerUpdated(NetworkIdentifier updatedNetworkEntity) {
        Debug.Log("OnPlayerUpdated: " + updatedNetworkEntity);

        if (!_hasInitialized) {
            return;
        }

        //Debug.Log(data);

        if (updatedNetworkEntity.Id == _myPlayerController.NetworkEntity.Oid) {
            Reconciliation(updatedNetworkEntity);
        }

        FillInterpolationBuffer(updatedNetworkEntity);
    }

    private void OnPlayerAttackReceived(SPlayerInput input) {
        Debug.Log("OnPlayerAttackReceived: " + input);
        
        if (MSPlayerEvent.Attack == input.PlayerEvent) {
            int attackerID = input.ObjectId;
            int targetID = input.TargetID;
            int damage = input.Damage;

            Debug.Log("AttackerID: " +  attackerID);
            Debug.Log("TargetID: " + targetID);
            Debug.Log("Damage: " + damage);

            PlayerController victim = null;

            //If attacker is my player and there is a target.
            if (targetID != 0 && attackerID == _myPlayerController.NetworkEntity.Oid) {
                _myPlayerController.Attack();
                victim = _otherPlayerControllers[targetID];
                victim.TakeDamage(damage);
            }
            //If attacker is my player and there is no target.
            else if (targetID == 0 && attackerID == _myPlayerController.NetworkEntity.Oid) {
                return;
            }
            //If victim is my player.
            else if (attackerID != 0 && targetID == _myPlayerController.NetworkEntity.Oid) {
                victim = _myPlayerController;
                _otherPlayerControllers[attackerID].SelectTarget(victim);
                _otherPlayerControllers[attackerID].Attack();
                victim.TakeDamage(damage);
            }
            //If there is no victim and attacker is not my player.
            else if (targetID == 0 && attackerID != _myPlayerController.NetworkEntity.Oid) {
                _otherPlayerControllers[attackerID].DeselectTarget();
                _otherPlayerControllers[attackerID].Attack();
            }
            //If victim is other player and attacker is not my player.
            else if (attackerID != 0 && targetID != _myPlayerController.NetworkEntity.Oid) {
                victim = _otherPlayerControllers[targetID];
                victim.TakeDamage(damage);
                _otherPlayerControllers[attackerID].SelectTarget(victim);
                _otherPlayerControllers[attackerID].Attack();
            }
        }
    }

    private void OnPlayerSpellReceived(ShiftServerData data ) {
        Debug.Log("OnPlayerSpellReceived: " + data);
    }

    private void Reconciliation(NetworkIdentifier updatedNetworkObject) {
        //_myPlayerController.transform.position = new Vector3(updatedPlayerObject.PosX, updatedPlayerObject.PosY, updatedPlayerObject.PosZ);

        //My Reconciliation
        if (NetworkManager.instance.Reconciliaton) {
            for (int jj = 0; jj < _myPlayerController.NetworkEntity.PlayerInputs.Count; jj++) {
                if (_myPlayerController.NetworkEntity.GetSequenceID(jj) <= updatedNetworkObject.LastProcessedInputID) {
                    _myPlayerController.NetworkEntity.RemoveRange(jj, 1);
                } else {
                    // re apply
                }
            }
        } else {
            _myPlayerController.NetworkEntity.ClearPlayerInputs();
        }
    }

    private void FillInterpolationBuffer(NetworkIdentifier updatedNetworkObject) {
        //Other Entity's Movement
        for (int jj = 0; jj < _otherPlayerControllers.Count; jj++) {
            PlayerController otherPlayerController = _otherPlayerControllers.ElementAt(jj).Value;
            if (otherPlayerController.NetworkEntity.Oid == updatedNetworkObject.Id) {
                if (Utils.IsValid(updatedNetworkObject.PositionX, updatedNetworkObject.PositionY, updatedNetworkObject.PositionZ)) {
                    Vector3 updatedPosition = new Vector3(updatedNetworkObject.PositionX.ToFloat(), updatedNetworkObject.PositionY.ToFloat(), updatedNetworkObject.PositionZ.ToFloat());

                    if (otherPlayerController.NetworkEntity.LastProcessedInputSequenceID <= updatedNetworkObject.LastProcessedInputID) {
                        DateTime updateTime = DateTime.UtcNow;
                        var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        if (otherPlayerController.NetworkEntity.PositionBuffer.Count == 0) {
                            otherPlayerController.NetworkEntity.AddPositionToBuffer(now, otherPlayerController.transform.position, updatedNetworkObject.LastProcessedInputID);
                        }
                        otherPlayerController.NetworkEntity.AddPositionToBuffer(now, updatedPosition, updatedNetworkObject.LastProcessedInputID);
                    }

                    otherPlayerController.NetworkEntity.LastProcessedInputSequenceID = updatedNetworkObject.LastProcessedInputID;
                }
            }
        }
    }

    private void ProcessInterpolation(double renderTimestamp, PlayerController entity) {
        // Drop older positions.
        while (entity.NetworkEntity.PositionBuffer.Count >= 2 && entity.NetworkEntity.PositionBuffer[1].updateTime <= renderTimestamp) {
            entity.NetworkEntity.PositionBuffer = entity.NetworkEntity.PositionBuffer.Skip(1).ToList();
        }

        // Interpolate between the two surrounding authoritative positions.
        if (entity.NetworkEntity.PositionBuffer.Count >= 2 && entity.NetworkEntity.PositionBuffer[0].updateTime <= renderTimestamp && renderTimestamp <= entity.NetworkEntity.PositionBuffer[1].updateTime) {
            Vector3 firstVector = entity.NetworkEntity.PositionBuffer[0].vector3;
            Vector3 secondVector = entity.NetworkEntity.PositionBuffer[1].vector3;

            double t0 = entity.NetworkEntity.PositionBuffer[0].updateTime;
            double t1 = entity.NetworkEntity.PositionBuffer[1].updateTime;

            double interpX = firstVector.x + (secondVector.x - firstVector.x) * (renderTimestamp - t0) / (t1 - t0);
            double interpZ = firstVector.z + (secondVector.z - firstVector.z) * (renderTimestamp - t0) / (t1 - t0);

            Vector3 newPosition = new Vector3((float)interpX, 0, (float)interpZ);

            entity.NetworkEntity.NetworkObject.PositionX = newPosition.x.ToString();
            entity.NetworkEntity.NetworkObject.PositionY = newPosition.y.ToString();
            entity.NetworkEntity.NetworkObject.PositionZ = newPosition.z.ToString();
        }
    }

    private void OnPlayerCreated(ShiftServerData data) {
        Debug.Log("OnPlayerCreated: " + data);
        
        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        if (playerInfo.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
            _myRoomPlayerInfo = playerInfo;
        } else {
            _otherPlayerRoomInfos.Add(playerInfo);
        }
    }

    private void OnRoomJoinSuccess(ShiftServerData data) {
        Debug.Log("OnRoomJoinSuccess: " + data);

        MSSRoom joinedRoom = data.RoomData.Room;

        for (int ii = 0; ii < data.RoomData.PlayerList.Count; ii++) {
            _otherPlayerRoomInfos.Add(data.RoomData.PlayerList[ii]);
        }

        onRoomJoined?.Invoke();
    }

    private void OnRoomJoinFailed(ShiftServerData data) {
        Debug.Log("OnRoomJoinFailed: " + data);
    }

    private void OnRoomCreated(ShiftServerData data) {
        Debug.Log("OnRoomCreated: " + data);

        //playerInfo = data.RoomData.CreatedRoom.Teams;

        //onRoomCreated?.Invoke();
    }

    private void OnRoomCreateFailed(ShiftServerData data) {
        Debug.Log("OnRoomCreateFailed: " + data);
    }

    private void OnRoomDeleted(ShiftServerData data) {
        Debug.Log("OnRoomDeleted: " + data);

        _myPlayerController = null;
        _otherPlayerControllers = new Dictionary<int, PlayerController>();
        _otherPlayerRoomInfos = new List<RoomPlayerInfo>();
    }

    private void OnRoomDeleteFailed(ShiftServerData data) {
        Debug.Log("OnRoomDeleteFailed: " + data);
    }

    private void OnRoomPlayerJoined(ShiftServerData data) {
        Debug.Log("OnRoomPlayerJoined: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        _otherPlayerRoomInfos.Add(data.RoomData.PlayerInfo);
        CreatePlayer(data.RoomData.PlayerInfo);
    }

    private void OnRoomPlayerLeft(ShiftServerData data) {
        Debug.Log("OnRoomPlayerLeft: " + data);

        RoomPlayerInfo playerInfo = data.RoomData.PlayerInfo;

        for (int ii = 0; ii < _otherPlayerControllers.Count; ii++) {
            PlayerController otherPlayerController = _otherPlayerControllers.ElementAt(ii).Value;

            if (playerInfo.NetworkObject.Id == otherPlayerController.NetworkEntity.Oid) {
                PlayerController leftPlayer = otherPlayerController;
                _otherPlayerControllers.Remove(leftPlayer.NetworkEntity.Oid);
                _otherPlayerRoomInfos.Remove(_otherPlayerRoomInfos.Where(roomPlayerInfo => roomPlayerInfo.NetworkObject.Id == leftPlayer.NetworkEntity.Oid).First());
                leftPlayer.Destroy();
                break;
            }
        }
    }

    private void OnRoomLeaveSuccess(ShiftServerData data) {
        Debug.Log("OnRoomLeaveSuccess: " + data);

        _myPlayerController = null;
        _otherPlayerControllers = new Dictionary<int, PlayerController>();
        _otherPlayerRoomInfos = new List<RoomPlayerInfo>();

        onRoomLeft?.Invoke();
    }

    private void OnRoomLeaveFailed(ShiftServerData data) {
        Debug.Log("OnRoomLeaveFailed: " + data);
    }

    private void OnRoomGetPlayers(ShiftServerData data) {
        Debug.Log("OnRoomGetPlayers: " + data);
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
