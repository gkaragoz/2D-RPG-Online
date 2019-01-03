using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyRow : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI _txtRowNumber;
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

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _rowNumber;

    private ServerRoom _serverRoom;

    public string GetRoomID {
        get {
            return _serverRoom.Id;
        }
    }

    public bool IsAvailableToJoin {
        get {
            return _serverRoom.CurrentUserCount <= _serverRoom.MaxUserCount && !_serverRoom.IsPrivate ? true : false;
        }
    }

    public void Initialize(string rowNumber, ServerRoom serverRoom) {
        this._serverRoom = serverRoom;

        SetRowNumber(rowNumber);
        SetRoomName();
        SetUserCount();
        SetPrivateToggle();
        SetActionButtonsInteractions();
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        _btnJoin.onClick.AddListener(() => joinDelegate(_serverRoom.Id));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        _btnWatch.onClick.AddListener(() => watchDelegate(_serverRoom.Id));
    }

    private void SetActionButtonsInteractions() {
        if (_serverRoom.IsOwner) {
            DeactivateJoinButtonInteraction();
            DeactivateWatchButtonInteraction();
        } else if (IsAvailableToJoin) {
            ActivateJoinButtonInteraction();
            ActivateWatchButtonInteraction();
        } else {
            DeactivateJoinButtonInteraction();
            DeactivateWatchButtonInteraction();
        }
    }

    private void ActivateJoinButtonInteraction() {
        _btnJoin.interactable = true;
    }

    private void DeactivateJoinButtonInteraction() {
        _btnJoin.interactable = false;
    }

    private void ActivateWatchButtonInteraction() {
        _btnWatch.interactable = true;
    }

    private void DeactivateWatchButtonInteraction() {
        _btnWatch.interactable = false;
    }

    private void SetRowNumber(string rowNumber) {
        _txtRowNumber.text = rowNumber;
    }

    private void SetRoomName() {
        _txtRoomName.text = _serverRoom.Name;
    }

    private void SetUserCount() {
        _txtPlayersCount.text = _serverRoom.CurrentUserCount + "/" + _serverRoom.MaxUserCount;
    }

    private void SetPrivateToggle() {
        _togglePrivate.isOn = _serverRoom.IsPrivate;
    }

}
