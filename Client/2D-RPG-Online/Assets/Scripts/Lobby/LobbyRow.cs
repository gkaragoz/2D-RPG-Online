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

    public void Initialize(int rowNumber, ServerRoom serverRoom) {
        this._serverRoom = serverRoom;
        this._rowNumber = rowNumber;
    }

    public void UpdateUI(string joinedRoomID) {
        SetRowNumber();
        SetRoomName();
        SetUserCount();
        SetPrivateToggle();

        SetActionButtonsInteractions(joinedRoomID);
    }

    public void SetReturnButtonOnClickAction(LobbyManager.ReturnDelegate returnDelegate) {
        _btnReturn.onClick.AddListener(() => returnDelegate(_serverRoom.Id));
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        _btnJoin.onClick.AddListener(() => joinDelegate(_serverRoom.Id));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        _btnWatch.onClick.AddListener(() => watchDelegate(_serverRoom.Id));
    }

    private void SetActionButtonsInteractions(string joinedRoomID) {
        if (joinedRoomID == GetRoomID || _serverRoom.IsOwner) {
            ActivateReturnButtonInteraction();
            DeactivateJoinButtonInteraction();
            DeactivateWatchButtonInteraction();
        } else if (IsAvailableToJoin) {
            DeactivateReturnButtonInteraction();
            ActivateJoinButtonInteraction();
            ActivateWatchButtonInteraction();
        }
    }

    private void ActivateReturnButtonInteraction() {
        _btnReturn.interactable = true;
    }

    private void DeactivateReturnButtonInteraction() {
        _btnReturn.interactable = false;
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

    private void SetRowNumber() {
        _txtRowNumber.text = _rowNumber.ToString();
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
