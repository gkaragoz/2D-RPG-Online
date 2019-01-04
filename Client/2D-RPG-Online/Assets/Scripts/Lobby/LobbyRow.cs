using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRow : MonoBehaviour {

    [Serializable]
    public class UISettings {
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

        public void UpdateUI(int rowNumber, MSSRoom mssRoom) {
            SetRowNumber(rowNumber);
            SetRoomName(mssRoom.Name);
            SetUserCount(mssRoom.CurrentUserCount, mssRoom.MaxUserCount);
            SetPrivateToggle(mssRoom.IsPrivate);
        }

        private void ActivateReturnButtonInteraction() {
            BtnReturn.interactable = true;
        }

        private void DeactivateReturnButtonInteraction() {
            BtnReturn.interactable = false;
        }

        private void ActivateJoinButtonInteraction() {
            BtnJoin.interactable = true;
        }

        private void DeactivateJoinButtonInteraction() {
            BtnJoin.interactable = false;
        }

        private void ActivateWatchButtonInteraction() {
            BtnWatch.interactable = true;
        }

        private void DeactivateWatchButtonInteraction() {
            BtnWatch.interactable = false;
        }

        private void SetRowNumber(int rowNumber) {
            _txtRowNumber.text = rowNumber.ToString();
        }

        private void SetRoomName(string roomName) {
            _txtRoomName.text = roomName;
        }

        private void SetUserCount(int currentUserCount, int maxUserCount) {
            _txtPlayersCount.text = currentUserCount + "/" + maxUserCount;
        }

        private void SetPrivateToggle(bool isPrivate) {
            _togglePrivate.isOn = isPrivate;
        }
    }

    [Header("Initialization")]
    [SerializeField]
    private UISettings _UISettings;

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

       _UISettings.UpdateUI(_rowNumber, _MSSRoom);
    }

    public void Initialize(int rowNumber) {
        this._rowNumber = rowNumber;

        _UISettings.UpdateUI(_rowNumber, _MSSRoom);
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    public void SetReturnButtonOnClickAction(LobbyManager.ReturnDelegate returnDelegate) {
        _UISettings.BtnReturn.onClick.AddListener(() => returnDelegate(_MSSRoom.Id));
    }

    public void SetJoinButtonOnClickAction(LobbyManager.JoinDelegate joinDelegate) {
        _UISettings.BtnJoin.onClick.AddListener(() => joinDelegate(_MSSRoom.Id));
    }

    public void SetWatchButtonOnClickAction(LobbyManager.WatchDelegate watchDelegate) {
        _UISettings.BtnWatch.onClick.AddListener(() => watchDelegate(_MSSRoom.Id));
    }

}
