using TMPro;
using UnityEngine;
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
    private Button _btnReturnRoom;
    [SerializeField]
    private Button _btnJoinRoom;
    [SerializeField]
    private Button _btnWatchRoom;

    public Button BtnReturnRoom {
        get {
            return _btnReturnRoom;
        }
    }

    public Button BtnJoinRoom {
        get {
            return _btnJoinRoom;
        }
    }

    public Button BtnWatchRoom {
        get {
            return _btnWatchRoom;
        }
    }

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _rowNumber;

    private MSSRoom _room;

    public string RoomID {
        get {
            return _room.Id;
        }
    }

    public void UpdateUI(int rowNumber, MSSRoom room) {
        this._room = room;
        this._rowNumber = rowNumber;

        SetRowNumber();
        SetRoomName();
        SetUserCount();
        SetPrivateToggle();
        ToggleActionButtonsVisibility();
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    public void SetReturnRoomButtonOnClickAction(LobbyManager.ReturnDelegate returnDelegate) {
        BtnReturnRoom.onClick.AddListener(() => returnDelegate(_room.Id));
    }

    public void SetJoinRoomButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        BtnJoinRoom.onClick.AddListener(() => joinDelegate(_room.Id));
    }

    public void SetWatchRoomButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        BtnWatchRoom.onClick.AddListener(() => watchDelegate(_room.Id));
    }

    public void SetJoinRoomButtonInteractions() {
        if (NetworkManager.mss.HasPlayerRoom) {
            BtnJoinRoom.interactable = false;
        } else if (!IsJoinedRoom() && IsAvailableToJoin()) {
            BtnJoinRoom.interactable = true;
        }
    }

    public void SetWatchRoomButtonInteractions() {
        if (NetworkManager.mss.HasPlayerRoom) {
            BtnWatchRoom.interactable = false;
        } else if (!IsJoinedRoom()) {
            BtnWatchRoom.interactable = true;
        }
    }

    private void ToggleActionButtonsVisibility() {
        if (NetworkManager.mss.HasPlayerRoom) {
            if (IsJoinedRoom()) {
                ShowReturnRoomButton();
            }
        } else {
            ShowJoinRoomButton();
        }
    }

    private void ShowReturnRoomButton() {
        BtnJoinRoom.gameObject.SetActive(false);
        BtnReturnRoom.gameObject.SetActive(true);

        SetJoinRoomButtonInteractions();
    }

    private void ShowJoinRoomButton() {
        BtnReturnRoom.gameObject.SetActive(false);
        BtnJoinRoom.gameObject.SetActive(true);

        SetJoinRoomButtonInteractions();
    }

    private void ActivateWatchRoomButtonInteraction() {
        BtnWatchRoom.interactable = true;
    }

    private void DeactivateWatchRoomButtonInteraction() {
        BtnWatchRoom.interactable = false;
    }

    private void SetRowNumber() {
        _txtRowNumber.text = _rowNumber.ToString();
    }

    private void SetRoomName() {
        _txtRoomName.text = _room.Name;
    }

    private void SetUserCount() {
        _txtPlayersCount.text = _room.CurrentUserCount + "/" + _room.MaxUserCount;
    }

    private void SetPrivateToggle() {
        _togglePrivate.isOn = _room.IsPrivate;
    }

    private bool IsJoinedRoom() {
        if (!NetworkManager.mss.HasPlayerRoom) return false;

        return NetworkManager.mss.JoinedRoom.Id == RoomID ? true : false;
    }

    private bool IsAvailableToJoin() {
        return _room.CurrentUserCount <= _room.MaxUserCount && !_room.IsPrivate ? true : false;
    }

}
