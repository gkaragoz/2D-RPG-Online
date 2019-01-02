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

    public int CurrentUsersCount {
        get {
            return _currentUsersCount;
        }

        set {
            _currentUsersCount = value;

            SetUsersCount();
        }
    }

    public int MaxUsersCount {
        get {
            return _maxUsersCount;
        }

        set {
            _maxUsersCount = value;

            SetUsersCount();
        }
    }

    public bool IsPrivate {
        get {
            return _isPrivate;
        }

        set {
            _isPrivate = value;

            SetPrivateToggle();
        }
    }

    public void SetJoinButtonOnClickAction(UnityAction action) {
        _btnJoin.onClick.AddListener(action);
    }

    public void SetWatchButtonOnClickAction(UnityAction action) {
        _btnWatch.onClick.AddListener(action);
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
