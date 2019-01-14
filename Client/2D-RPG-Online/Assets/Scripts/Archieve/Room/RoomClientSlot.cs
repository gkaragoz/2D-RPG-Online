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
    private Toggle _toggleIsReady;
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
        SetToggleIsReady();
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

    private void SetToggleIsReady() {
        _toggleIsReady.isOn = _playerInfo.IsReady;
    }

    private void SetImgLeader() {
        _imgLeader.gameObject.SetActive(_playerInfo.IsLeader);
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
