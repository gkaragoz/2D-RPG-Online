
using TMPro;
using UnityEngine;

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
        AccountManager.instance.onAccountUpdated = UpdateUI;
    }

    public void Initialize() {
        if (CharacterManager.instance.HasCharacter) {
            UpdateUI();
        }

        initializationProgress.Invoke();
    }

    private void UpdateUI() {
        _txtAccountGold.text = AccountManager.instance.Account.gold.ToString();
        _txtAccountGem.text = AccountManager.instance.Account.gem.ToString();

        if (CharacterManager.instance.HasCharacter) {
            _txtCharacterName.text = CharacterManager.instance.SelectedCharacter.name.ToString();
            _txtCharacterLevel.text = CharacterManager.instance.SelectedCharacter.level.ToString();
            _txtCharacterExperience.text = CharacterManager.instance.SelectedCharacter.exp.ToString();
        }

        //_txtCharacterStatPoints.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterHealth.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterMana.text = AccountManager.instance.Account.gold.ToString();

        //_txtCharacterStrength.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterIntelligence.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterDexterity.text = AccountManager.instance.Account.gold.ToString();
    }

}
