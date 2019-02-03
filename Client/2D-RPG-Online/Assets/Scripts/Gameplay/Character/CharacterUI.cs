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
    [SerializeField]
    private GameObject _targetIndicatorObj;

    private CharacterStats _characterStats;

    private void Awake() {
        _characterStats = GetComponent<CharacterStats>();
    }

    public void UpdateUI() {
        SetName();
        SetHealthBar();
        SetManaBar();
    }

    public void OpenUI() {
        Show();

        _targetIndicatorObj.SetActive(true);
    }

    public void HideUI() {
        Hide();

        _targetIndicatorObj.SetActive(false);
    }

    private void SetName() {
        _txtName.text = _characterStats.GetName();
    }

    private void SetHealthBar() {
        _imgHealthBar.fillAmount = ((float)_characterStats.GetCurrentHealth() / (float)_characterStats.GetMaxHealth());

        if (_imgHealthBar.fillAmount == 0) {
            _imgHealthBar.transform.parent.gameObject.SetActive(false);
            _imgManaBar.transform.parent.gameObject.SetActive(false);
        }
    }

    private void SetManaBar() {
        _imgManaBar.fillAmount = (_characterStats.GetCurrentMana() / _characterStats.GetMaxMana());
    }

}
