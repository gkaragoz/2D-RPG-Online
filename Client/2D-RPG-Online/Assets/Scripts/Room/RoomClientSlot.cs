using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class RoomClientSlot : MonoBehaviour {

    [Serializable]
    public class UISettings {
        public Image imgFrame;
        public Image imgCharacter;
        public Image imgClassIcon;
        public Toggle toggleIsReady;
        public Image imgLeader;
        public TextMeshProUGUI txtPlayerName;
        public TextMeshProUGUI txtCharacterName;

        [Header("Debug")]
        [SerializeField]
        [Utils.ReadOnly]
        private AnimatorController _animatorController;

        public void SetCharacterClassVisualize(ClassManager.Classes characterClass) {
            ClassManager.CharacterClassVisualization characterClassVisualization = ClassManager.instance.GetCharacterClassVisualize(characterClass);

            SetImgFrameColor(characterClassVisualization.classFrameColor);
            SetImgCharacter(characterClassVisualization.classSprite);
            SetAnimatorController(characterClassVisualization.classAnimatorController);
            SetImgCharacter(characterClassVisualization.classIcon);
            SetTxtCharacterName(characterClassVisualization.className);
        }

        public void UpdateUI(RoomPlayerInfo roomPlayerInfo) {
            SetToggleIsReady(true);
            SetImgLeader(true);
            SetTxtPlayerName(roomPlayerInfo.Username);
        }

        private void SetToggleIsReady(bool isReady) {
            toggleIsReady.isOn = isReady;
        }

        private void SetImgLeader(bool isLeader) {
            imgLeader.enabled = isLeader;
        }

        private void SetTxtPlayerName(string playerName) {
            txtPlayerName.text = playerName;
        }

        private void SetImgFrameColor(Color color) {
            imgFrame.color = color;
        }

        private void SetImgCharacter(Sprite sprite) {
            imgCharacter.sprite = sprite;
        }

        private void SetAnimatorController(AnimatorController characterAnimator) {
            _animatorController = characterAnimator;
        }

        private void SetImgClassIcon(Sprite sprite) {
            imgClassIcon.sprite = sprite;
        }

        private void SetTxtCharacterName(string name) {
            txtPlayerName.text = name;
        }
    }

    [Header("Initialization")]
    [SerializeField]
    private UISettings _UISettings;

    public string Username {
        get {
            return _roomPlayerInfo.Username;
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private ClassManager.Classes _classEnum;

    public ClassManager.Classes ClassEnum {
        get {
            return _classEnum;
        }

        set {
            _classEnum = value;

            _UISettings.SetCharacterClassVisualize(ClassEnum);
        }
    }

    private RoomPlayerInfo _roomPlayerInfo;

    public void Initialize(RoomPlayerInfo roomPlayerInfo) {
        this._roomPlayerInfo = roomPlayerInfo;

        _UISettings.UpdateUI(_roomPlayerInfo);
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

}
