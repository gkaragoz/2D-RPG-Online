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
    private Button _btnReturn;
    [SerializeField]
    private Button _btnJoin;
    [SerializeField]
    private Button _btnWatch;

    public Button BtnReturn {
        get {
            return _btnReturn;
        }
    }

    public Button BtnJoin {
        get {
            return _btnJoin;
        }
    }

    public Button BtnWatch {
        get {
            return _btnWatch;
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

    public void SetReturnButtonOnClickAction(LobbyManager.ReturnDelegate returnDelegate) {
        BtnReturn.onClick.AddListener(() => returnDelegate(_room.Id));
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        BtnJoin.onClick.AddListener(() => joinDelegate(_room.Id));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        BtnWatch.onClick.AddListener(() => watchDelegate(_room.Id));
    }

    private void ToggleActionButtonsVisibility() {
        if (NetworkManager.mss.HasPlayerRoom) {
            if (IsJoinedRoom()) {
                ShowReturnButton();
            }
        } else {
            ShowJoinButton();
        }
    }

    private void SetJoinButtonVisibility() {
        if (NetworkManager.mss.HasPlayerRoom) {
            if (!IsJoinedRoom() && IsAvailableToJoin()) {
                BtnJoin.gameObject.SetActive(true);
            } else {
                BtnJoin.gameObject.SetActive(false);
            }
        } else if (IsAvailableToJoin()) {
            BtnJoin.gameObject.SetActive(true);
        }
    }

    private void ShowReturnButton() {
        BtnJoin.gameObject.SetActive(false);
        BtnReturn.gameObject.SetActive(true);

        SetJoinButtonVisibility();
    }

    private void ShowJoinButton() {
        BtnReturn.gameObject.SetActive(false);
        BtnJoin.gameObject.SetActive(true);

        SetJoinButtonVisibility();
    }

    private void ActivateWatchButtonInteraction() {
        BtnWatch.interactable = true;
    }

    private void DeactivateWatchButtonInteraction() {
        BtnWatch.interactable = false;
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
        return NetworkManager.mss.JoinedRoom.Id == _room.Id ? true : false;
    }

    private bool IsAvailableToJoin() {
        return _room.CurrentUserCount <= _room.MaxUserCount && !_room.IsPrivate ? true : false;
    }

}
