﻿
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Menu {

    #region Singleton

    public static MenuManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public LoadingTask initializationProgress;

    [SerializeField]
    private Image _imgClassBg;

    [SerializeField]
    private TextMeshProUGUI _txtAccountGold;
    [SerializeField]
    private TextMeshProUGUI _txtAccountGem;

    [SerializeField]
    private TextMeshProUGUI _txtCharacterName;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterLevel;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterExperience;

    [SerializeField]
    private TextMeshProUGUI _txtCharacterStatPoints;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterHealth;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterMana;

    [SerializeField]
    private TextMeshProUGUI _txtCharacterStrength;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterIntelligence;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterDexterity;

    private void Start() {
        AccountManager.instance.onAccountUpdated += UpdateUI;
    }

    public void Initialize() {
        if (AccountManager.instance.HasCharacter) {
            UpdateUI();
        }

        initializationProgress.Invoke();
    }

    private void UpdateUI() {
        _txtAccountGold.text = AccountManager.instance.Gold.ToString();
        _txtAccountGem.text = AccountManager.instance.Gem.ToString();

        if (AccountManager.instance.HasCharacter) {
            _imgClassBg.color = CharacterClassVisualization.instance.GetVisualizationProperties((CharacterClassVisualization.Classes)CharacterManager.instance.SelectedCharacter.class_index).ClassFrameColor;

            _txtCharacterName.text = CharacterManager.instance.SelectedCharacter.name.ToString();
            _txtCharacterLevel.text = CharacterManager.instance.SelectedCharacter.level + " Lv.";
            _txtCharacterExperience.text = "Exp: " + CharacterManager.instance.SelectedCharacter.exp;

            _txtCharacterStatPoints.text = "x" + CharacterManager.instance.SelectedCharacter.stat_points;

            _txtCharacterHealth.text = CharacterManager.instance.SelectedCharacter.stat.health.ToString();
            _txtCharacterMana.text = CharacterManager.instance.SelectedCharacter.stat.mana.ToString();

            _txtCharacterStrength.text = CharacterManager.instance.SelectedCharacter.attribute.strength.ToString();
            _txtCharacterIntelligence.text = CharacterManager.instance.SelectedCharacter.attribute.intelligence.ToString();
            _txtCharacterDexterity.text = CharacterManager.instance.SelectedCharacter.attribute.dexterity.ToString();
        }
    }

}
