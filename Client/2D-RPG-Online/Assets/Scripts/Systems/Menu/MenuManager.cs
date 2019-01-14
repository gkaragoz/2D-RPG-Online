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

    public Task initializationProgress;

    [SerializeField]
    private Button _btnCreateRoom;
    [SerializeField]
    private Button _btnNormalGame;
    [SerializeField]
    private Button _btnRankedGame;

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
    [SerializeField]
    private Image _imgCharacterStrengthBar;
    [SerializeField]
    private Image _imgCharacterIntelligenceBar;
    [SerializeField]
    private Image _imgCharacterDexterityBar;

    private void Start() {
        AccountManager.instance.onAccountUpdated += UpdateUI;
    }

    public void Initialize() {
        if (CharacterManager.instance.HasCharacter) {
            UpdateUI();
        }

        initializationProgress.Invoke();
    }

    public void SetInteractionOfCreateRoomButton(bool interactable) {
        _btnCreateRoom.interactable = interactable;
    }

    public void SetInteractionOfNormalGameButton(bool interactable) {
        _btnNormalGame.interactable = interactable;
    }

    public void SetInteractionOfRankedGameButton(bool interactable) {
        _btnRankedGame.interactable = interactable;
    }

    private void UpdateUI() {
        _txtAccountGold.text = AccountManager.instance.Account.gold.ToString();
        _txtAccountGem.text = AccountManager.instance.Account.gem.ToString();

        if (CharacterManager.instance.HasCharacter) {
            _txtCharacterName.text = CharacterManager.instance.SelectedCharacter.name.ToString();
            _txtCharacterLevel.text = CharacterManager.instance.SelectedCharacter.level + " Lv.";
            _txtCharacterExperience.text = "Exp: " + CharacterManager.instance.SelectedCharacter.exp;

            _txtCharacterStatPoints.text = "x" + CharacterManager.instance.SelectedCharacter.stat_points;

            _txtCharacterHealth.text = CharacterManager.instance.SelectedCharacter.stat.health.ToString();
            _txtCharacterMana.text = CharacterManager.instance.SelectedCharacter.stat.mana.ToString();

            _txtCharacterStrength.text = CharacterManager.instance.SelectedCharacter.attribute.strength.ToString();
            _txtCharacterIntelligence.text = CharacterManager.instance.SelectedCharacter.attribute.intelligence.ToString();
            _txtCharacterDexterity.text = CharacterManager.instance.SelectedCharacter.attribute.dexterity.ToString();
            _imgCharacterStrengthBar.fillAmount = CharacterManager.instance.SelectedCharacter.attribute.strength * 0.01f;
            _imgCharacterIntelligenceBar.fillAmount = CharacterManager.instance.SelectedCharacter.attribute.intelligence * 0.01f;
            _imgCharacterDexterityBar.fillAmount = CharacterManager.instance.SelectedCharacter.attribute.dexterity * 0.01f;
        }
    }

}
