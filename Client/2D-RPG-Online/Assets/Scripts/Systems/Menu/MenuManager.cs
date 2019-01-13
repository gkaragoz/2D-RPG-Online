using System.Collections;
using System.Collections.Generic;
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

    public void Initialize() {
        UpdateUI();

        initializationProgress?.Invoke();
    }

    private void UpdateUI() {
        _txtAccountGold.text = AccountManager.instance.Account.gold.ToString();
        _txtAccountGem.text = AccountManager.instance.Account.gem.ToString();

        //_txtCharacterName.text = AccountManager.instance.Account.na.ToString();
        //_txtCharacterLevel.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterExperience.text = AccountManager.instance.Account.gold.ToString();

        //_txtCharacterStatPoints.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterHealth.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterMana.text = AccountManager.instance.Account.gold.ToString();

        //_txtCharacterStrength.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterIntelligence.text = AccountManager.instance.Account.gold.ToString();
        //_txtCharacterDexterity.text = AccountManager.instance.Account.gold.ToString();
    }

}
