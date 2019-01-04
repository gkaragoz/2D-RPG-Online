using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    #region Singleton

    public static TeamManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    private List<Team> _teamList = new List<Team>();

    public void CreateTeam(string[] ids) {
        for (int ii = 0; ii < ids.Length; ii++) {
            Team team = new Team(ids[ii], "New Team");

            _teamList.Add(team);
        }
    }

    public void CreateTeam(string[] ids, string[] names) {
        for (int ii = 0; ii < ids.Length; ii++) {
            Team team = new Team(ids[ii], names[ii]);

            _teamList.Add(team);
        }
    }

    public void AddPlayerToTeam(RoomPlayerInfo playerInfo) {
        if (IsTeamExists(playerInfo.TeamId)) {
            for (int ii = 0; ii < _teamList.Count; ii++) {
                if (_teamList[ii].ID == playerInfo.TeamId) {
                    _teamList[ii].AddPlayer(playerInfo);
                }
            }
        } else {
            LogManager.instance.AddLog("Adding player to team has failed!", Log.Type.Loot);
            LogManager.instance.AddLog("ERRORTYPE: Team not found! " + playerInfo.TeamId, Log.Type.Loot);
        }
    }

    public void RemovePlayerFromTeam(RoomPlayerInfo playerInfo) {
        if (IsTeamExists(playerInfo.TeamId)) {
            for (int ii = 0; ii < _teamList.Count; ii++) {
                if (_teamList[ii].ID == playerInfo.TeamId) {
                    _teamList[ii].RemovePlayer(playerInfo);
                }
            }
        } else {
            LogManager.instance.AddLog("Removing player from team has failed!", Log.Type.Loot);
            LogManager.instance.AddLog("ERRORTYPE: Team not found! " + playerInfo.TeamId, Log.Type.Loot);
        }
    }

    private bool IsTeamExists(string id) {
        for (int ii = 0; ii < _teamList.Count; ii++) {
            if (_teamList[ii].ID == id) {
                return true;
            }
        }
        return false;
    }

}
