using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomClientSlot : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private GameObject _emptySlotPrefab;
    [SerializeField]
    private GameObject _filledSlotPrefab;

    [SerializeField]
    private Image _imgFrame;
    [SerializeField]
    private Image _imgCharacter;
    [SerializeField]
    private Image _imgClassIcon;
    [SerializeField]
    private Image _imgReadyIcon;
    [SerializeField]
    private Image _imgNotReadyIcon;
    [SerializeField]
    private Image _imgLeader;
    [SerializeField]
    private TextMeshProUGUI _txtPlayerName;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterName;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private string _teamID;

    public string Username {
        get {
            return _playerInfo.Username;
        }
    }

    public bool IsFilledSlot {
        get {
            return _playerInfo == null ? false : true;
        }
    }

    public bool IsLeader {
        get {
            return _playerInfo.IsLeader;
        }
    }

    public bool IsReady {
        get {
            return _playerInfo.IsReady;
        }
    }

    public string TeamID {
        get {
            return _teamID;
        }

        set {
            _teamID = value;
        }
    }

    private CharacterClassVisualization.Properties _characterClassVisualization;

    private RoomPlayerInfo _playerInfo;

    public void UpdateUI(RoomPlayerInfo playerInfo) {
        this._playerInfo = playerInfo;
        this._characterClassVisualization = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Warrior);

        SetCharacterClassVisualize();
        SetIsReady();
        SetImgLeader();
        SetTxtPlayerName();

        if (IsFilledSlot) {
            ActivateFilledSlotPrefab();
        } else {
            Clear();
        }
    }

    public void Clear() {
        this._playerInfo = null;

        ActivateEmptySlotPrefab();
    }

    private void ActivateEmptySlotPrefab() {
        _filledSlotPrefab.SetActive(false);
        _emptySlotPrefab.SetActive(true);
    }

    private void ActivateFilledSlotPrefab() {
        _emptySlotPrefab.SetActive(false);
        _filledSlotPrefab.SetActive(true);
    }

    private void SetCharacterClassVisualize() {
        SetImgFrameColor();
        SetImgCharacter();
        SetImgClassIcon();
        SetTxtCharacterName();
    }

    private void SetIsReady() {
        if (IsLeader) {
            _imgNotReadyIcon.gameObject.SetActive(false);
            _imgReadyIcon.gameObject.SetActive(false);
            return;
        }

        if (IsReady) {
            _imgNotReadyIcon.gameObject.SetActive(false);
            _imgReadyIcon.gameObject.SetActive(true);
        } else {
            _imgReadyIcon.gameObject.SetActive(false);
            _imgNotReadyIcon.gameObject.SetActive(true);
        }
    }

    private void SetImgLeader() {
        _imgLeader.gameObject.SetActive(IsLeader);
    }

    private void SetTxtPlayerName() {
        _txtPlayerName.text = Username;
    }

    private void SetTxtCharacterName() {
        _txtPlayerName.text = _characterClassVisualization.ClassName;
    }

    private void SetImgFrameColor() {
        _imgFrame.color = _characterClassVisualization.ClassFrameColor;
    }

    private void SetImgCharacter() {
        _imgCharacter.sprite = _characterClassVisualization.ClassSprite;
    }

    private void SetImgClassIcon() {
        _imgClassIcon.sprite = _characterClassVisualization.ClassIcon;
    }

}
