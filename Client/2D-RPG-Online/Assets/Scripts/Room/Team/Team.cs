using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team {

    [SerializeField]
    [Utils.ReadOnly]
    private string _ID;
    [SerializeField]
    [Utils.ReadOnly]
    private string _name;
    [SerializeField]
    private List<RoomPlayerInfo> _playerList = new List<RoomPlayerInfo>();

    public string ID {
        get {
            return _ID;
        }

        private set {
            _ID = value;
        }
    }

    public string Name {
        get {
            return _name;
        }

        set {
            _name = value;
        }
    }

    public Team(string id, string name) {
        this.ID = id;
        this.Name = name;
    }

    public void AddPlayer(RoomPlayerInfo playerInfo) {
        if (!IsPlayerExists(playerInfo)) {
            _playerList.Add(playerInfo);
        } else {
            LogManager.instance.AddLog("Player(" + playerInfo.Username + ") already exist on team " + Name, Log.Type.Loot);
        }
    }

    public void RemovePlayer(RoomPlayerInfo playerInfo) {
        _playerList.Remove(playerInfo);
    }

    private bool IsPlayerExists(RoomPlayerInfo playerInfo) {
        for (int ii = 0; ii < _playerList.Count; ii++) {
            if (_playerList[ii].Username == playerInfo.Username) {
                return true;
            }
        }
        return false;
    }

}
