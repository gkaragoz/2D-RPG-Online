using System;
using UnityEngine;

public class CharacterClassVisualization : MonoBehaviour {

    #region Singleton

    public static CharacterClassVisualization instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public enum Classes {
        Warrior,
        Archer,
        Mage,
        Priest
    }

    [Serializable]
    public class Properties {
        [SerializeField]
        private string _className;
        [SerializeField]
        private string _classDescription;
        [SerializeField]
        private Sprite _classSprite;
        [SerializeField]
        private Color _classFrameColor;
        [SerializeField]
        private Sprite _classIcon;

        public string ClassName { get { return _className; } }
        public string ClassDescription { get { return _classDescription; } }
        public Sprite ClassSprite { get { return _classSprite; } }
        public Color ClassFrameColor { get { return _classFrameColor; } }
        public Sprite ClassIcon { get { return _classIcon; } }
    }

    [SerializeField]
    private Properties[] _characterClassVisualization;

    public Properties GetCharacterClassVisualize(Classes characterClass) {
        switch (characterClass) {
            case Classes.Warrior:
                return _characterClassVisualization[0];
            case Classes.Archer:
                return _characterClassVisualization[1];
            case Classes.Mage:
                return _characterClassVisualization[2];
            case Classes.Priest:
                return _characterClassVisualization[3];
            default:
                return null;
        }
    }

}
