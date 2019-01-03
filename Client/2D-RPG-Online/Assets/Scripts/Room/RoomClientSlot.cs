using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomClientSlot : MonoBehaviour {

    [Serializable]
    public class UISettings {
        public Image imgFrame;
        public Image imgCharacter;
        public Image imgClassIcon;
        public Toggle isReady;
        public Image imgLeader;
        public TextMeshProUGUI txtPlayerName;
        public TextMeshProUGUI txtCharacterName;
    }

    [Header("Initialization")]
    [SerializeField]
    private UISettings _UISettings;

}
