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
        public AnimatorController animatorController;
        public Toggle toggleIsReady;
        public Image imgLeader;
        public TextMeshProUGUI txtPlayerName;
        public TextMeshProUGUI txtCharacterName;

        public void Initialize(ClassManager.Classes characterClass) {
            ClassManager.CharacterClassVisualization characterClassVisualization = ClassManager.instance.GetCharacterClassVisualize(characterClass);

            SetImgFrameColor(characterClassVisualization.classFrameColor);
            SetImgCharacter(characterClassVisualization.classSprite);
            SetAnimatorController(characterClassVisualization.classAnimatorController);
            SetImgCharacter(characterClassVisualization.classIcon);
            SetTxtCharacterName(characterClassVisualization.className);
        }

        public void SetToggleIsReady(bool isReady) {
            toggleIsReady.isOn = isReady;
        }

        public void SetImgLeader(bool isLeader) {
            imgLeader.enabled = isLeader;
        }

        public void SetTxtPlayerName(string playerName) {
            txtPlayerName.text = playerName;
        }

        private void SetImgFrameColor(Color color) {
            imgFrame.color = color;
        }

        private void SetImgCharacter(Sprite sprite) {
            imgCharacter.sprite = sprite;
        }

        private void SetAnimatorController(AnimatorController characterAnimator) {
            animatorController = characterAnimator;
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

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _slotIndex;
    [SerializeField]
    [Utils.ReadOnly]
    private ClassManager.Classes _classEnum;
    [SerializeField]
    [Utils.ReadOnly]
    private bool _isReady;
    [SerializeField]
    [Utils.ReadOnly]
    private string _playerName;

    public int SlotIndex {
        get {
            return _slotIndex;
        }

        set {
            _slotIndex = value;
        }
    }

    public ClassManager.Classes ClassEnum {
        get {
            return _classEnum;
        }

        set {
            _classEnum = value;

            _UISettings.Initialize(ClassEnum);
        }
    }

    public bool IsReady {
        get {
            return _isReady;
        }

        set {
            _isReady = value;

            _UISettings.SetToggleIsReady(IsReady);
        }
    }

    public string PlayerName {
        get {
            return _playerName;
        }

        set {
            _playerName = value;

            _UISettings.SetTxtPlayerName(PlayerName);
        }
    }
}
