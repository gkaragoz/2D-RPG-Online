using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomClientSlot : MonoBehaviour {

    [Serializable]
    public class UISettings {
        public GameObject emptySlotPrefab;
        public GameObject filledSlotPrefab;

        public Image imgFrame;
        public Image imgCharacter;
        public Image imgClassIcon;
        public Toggle toggleIsReady;
        public Image imgLeader;
        public TextMeshProUGUI txtPlayerName;
        public TextMeshProUGUI txtCharacterName;

        public void UpdateUI(RoomPlayerInfo roomPlayerInfo, bool fill) {
            SetCharacterClassVisualize(ClassManager.Classes.Warrior);
            SetToggleIsReady(true);
            SetImgLeader(true);
            SetTxtPlayerName(roomPlayerInfo.Username);

            if (fill) {
                ActivateFilledSlotPrefab();
            } else {
                ClearUI();
            }
        }

        public void ClearUI() {
            ActivateEmptySlotPrefab();
        }

        private void ActivateEmptySlotPrefab() {
            filledSlotPrefab.SetActive(false);
            emptySlotPrefab.SetActive(true);
        }
        
        private void ActivateFilledSlotPrefab() {
            emptySlotPrefab.SetActive(false);
            filledSlotPrefab.SetActive(true);
        }

        private void SetCharacterClassVisualize(ClassManager.Classes characterClass) {
            ClassManager.CharacterClassVisualization characterClassVisualization = ClassManager.instance.GetCharacterClassVisualize(characterClass);

            SetImgFrameColor(characterClassVisualization.classFrameColor);
            SetImgCharacter(characterClassVisualization.classSprite);
            SetImgCharacter(characterClassVisualization.classIcon);
            SetTxtCharacterName(characterClassVisualization.className);
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

        private void SetTxtCharacterName(string name) {
            txtPlayerName.text = name;
        }

        private void SetImgFrameColor(Color color) {
            imgFrame.color = color;
        }

        private void SetImgCharacter(Sprite sprite) {
            imgCharacter.sprite = sprite;
        }

        private void SetImgClassIcon(Sprite sprite) {
            imgClassIcon.sprite = sprite;
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

    public bool IsFilledSlot {
        get {
            return _roomPlayerInfo == null ? false : true;
        }
    }
   
    private RoomPlayerInfo _roomPlayerInfo;

    public void Initialize(RoomPlayerInfo roomPlayerInfo) {
        this._roomPlayerInfo = roomPlayerInfo;

        _UISettings.UpdateUI(_roomPlayerInfo, IsFilledSlot);
    }

    public void Clear() {
        this._roomPlayerInfo = null;

        _UISettings.ClearUI();
    }

}
