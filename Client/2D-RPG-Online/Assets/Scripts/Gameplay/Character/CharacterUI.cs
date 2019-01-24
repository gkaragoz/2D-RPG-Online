using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : Menu {

    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private Image _imgHealthBar;
    [SerializeField]
    private Image _imgManaBar;

    private CharacterStats _characterStats;

    private void Start() {
        _characterStats = GetComponent<CharacterStats>();

        UpdateUI();
    }

    public void UpdateUI() {
        SetName();
        SetHealthBar();
        SetManaBar();
    }

    public void SetName() {
        _txtName.text = _characterStats.name;
    }

    public void SetHealthBar() {
        _imgHealthBar.fillAmount = (_characterStats.GetCurrentHealth() / _characterStats.GetMaxHealth());
    }

    public void SetManaBar() {
        _imgManaBar.fillAmount = (_characterStats.GetCurrentMana() / _characterStats.GetMaxMana());
    }

}
