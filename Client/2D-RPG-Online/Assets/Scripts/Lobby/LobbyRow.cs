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

    private MSSRoom _MSSRoom;

    public string GetRoomID {
        get {
            return _MSSRoom.Id;
        }
    }

    public bool IsAvailableToJoin {
        get {
            return _MSSRoom.CurrentUserCount <= _MSSRoom.MaxUserCount && !_MSSRoom.IsPrivate ? true : false;
        }
    }

    public void Initialize(int rowNumber, MSSRoom MSSRoom) {
        this._MSSRoom = MSSRoom;
        this._rowNumber = rowNumber;

        UpdateUI();
    }

    public void UpdateUI() {
        SetRowNumber();
        SetRoomName();
        SetUserCount();
        SetPrivateToggle();

        //SetActionButtonsInteractions(joinedRoomID);
    }

    public void SetReturnButtonOnClickAction(LobbyManager.ReturnDelegate returnDelegate) {
        _btnReturn.onClick.AddListener(() => returnDelegate(_MSSRoom.Id));
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        _btnJoin.onClick.AddListener(() => joinDelegate(_MSSRoom.Id));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        _btnWatch.onClick.AddListener(() => watchDelegate(_MSSRoom.Id));
    }

    private void SetActionButtonsInteractions(string joinedRoomID) {
        if (joinedRoomID == GetRoomID || _MSSRoom.IsOwner) {
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
        _txtRoomName.text = _MSSRoom.Name;
    }

    private void SetUserCount() {
        _txtPlayersCount.text = _MSSRoom.CurrentUserCount + "/" + _MSSRoom.MaxUserCount;
    }

    private void SetPrivateToggle() {
        _togglePrivate.isOn = _MSSRoom.IsPrivate;
    }

}
