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
    private string _rowNumber;
    [SerializeField]
    [Utils.ReadOnly]
    private string _roomID;
    [SerializeField]
    [Utils.ReadOnly]
    private string _roomName;
    [SerializeField]
    [Utils.ReadOnly]
    private bool _isOwner;
    [SerializeField]
    [Utils.ReadOnly]
    private long _createdTime;
    [SerializeField]
    [Utils.ReadOnly]
    private long _updatedTime;
    [SerializeField]
    [Utils.ReadOnly]
    private int _currentUsersCount;
    [SerializeField]
    [Utils.ReadOnly]
    private int _maxUsersCount;
    [SerializeField]
    [Utils.ReadOnly]
    private bool _isPrivate;

    public string RowNumber {
        get {
            return _rowNumber;
        }

        set {
            _rowNumber = value;

            SetRoomNumber();
        }
    }

    public string RoomID {
        get {
            return _roomID;
        }

        set {
            _roomID = value;
        }
    }

    public string RoomName {
        get {
            return _roomName;
        }

        set {
            _roomName = value;

            SetRoomName();
        }
    }

    public bool IsOwner {
        get {
            return _isOwner;
        }

        set {
            _isOwner = value;

            SetActionButtonsInteractions();
        }
    }

    public long CreatedTime {
        get {
            return _createdTime;
        }

        set {
            _createdTime = value;
        }
    }

    public long UpdatedTime {
        get {
            return _updatedTime;
        }

        set {
            _updatedTime = value;
        }
    }

    public int CurrentUsersCount {
        get {
            return _currentUsersCount;
        }

        set {
            _currentUsersCount = value;

            SetUsersCount();
            SetActionButtonsInteractions();
        }
    }

    public int MaxUsersCount {
        get {
            return _maxUsersCount;
        }

        set {
            _maxUsersCount = value;

            SetUsersCount();
            SetActionButtonsInteractions();
        }
    }

    public bool IsPrivate {
        get {
            return _isPrivate;
        }

        set {
            _isPrivate = value;

            SetPrivateToggle();
            SetActionButtonsInteractions();
        }
    }

    public bool IsAvailableToJoin {
        get {
            return CurrentUsersCount <= MaxUsersCount && !IsPrivate ? true : false;
        }
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        _btnJoin.onClick.AddListener(() => joinDelegate(RoomID));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        _btnWatch.onClick.AddListener(() => watchDelegate(RoomID));
    }

    private void SetActionButtonsInteractions() {
        if (IsOwner) {
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

    private void SetRoomNumber() {
        _txtRowNumber.text = RowNumber;
    }

    private void SetRoomName() {
        _txtRoomName.text = RoomName;
    }

    private void SetUsersCount() {
        _txtPlayersCount.text = CurrentUsersCount + "/" + MaxUsersCount;
    }

    private void SetPrivateToggle() {
        _togglePrivate.isOn = IsPrivate;
    }

}
