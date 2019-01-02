using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyGridRow : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _txtRoomID;
    [SerializeField]
    private TextMeshProUGUI _txtRoomName;
    [SerializeField]
    private TextMeshProUGUI _txtPlayersCount;
    [SerializeField]
    private Toggle _togglePrivate;
    [SerializeField]
    private Button _btnJoin;
    [SerializeField]
    private Button _btnWatch;

    public void SetRoomID(string ID) {
        _txtRoomID.text = ID;
    }

    public void SetRoomName(string roomName) {
        _txtRoomName.text = roomName;
    }

    public void SetPlayersCount(int currentPlayer, int maxPlayer) {
        _txtPlayersCount.text = currentPlayer + "/" + maxPlayer;
    }

    public void SetPrivateToggle(bool isPrivate) {
        _togglePrivate.isOn = isPrivate;
    }

    public void SetJoinButtonOnClickAction(UnityAction action) {
        _btnJoin.onClick.AddListener(action);
    }

    public void SetWatchButtonOnClickAction(UnityAction action) {
        _btnWatch.onClick.AddListener(action);
    }

}
