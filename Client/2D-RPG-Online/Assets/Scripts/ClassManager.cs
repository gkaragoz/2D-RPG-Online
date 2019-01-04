using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for collect user inputs about selected class.
/// </summary>
public class ClassManager : Menu {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static ClassManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
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
    public class CharacterClassVisualization {
        public string className;
        public Sprite classSprite;
        public Color classFrameColor;
        public Sprite classIcon;
    }

    [SerializeField]
    private CharacterClassVisualization[] _characterClassVisualization;

    public CharacterClassVisualization GetCharacterClassVisualize(Classes characterClass) {
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
