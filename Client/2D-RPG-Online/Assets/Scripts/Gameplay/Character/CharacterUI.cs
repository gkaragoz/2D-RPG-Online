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
    [SerializeField]
    private TargetUI _targetUI;

    private CharacterStats _characterStats;

    private void Awake() {
        _characterStats = GetComponent<CharacterStats>();
    }

    public void UpdateUI() {
        SetName();
        SetHealthBar();
        SetManaBar();
    }

    public void UpdateTargetUI() {
        //NPC, MOB, OBJECT don't have target UI.
        if (_targetUI != null) {
            _targetUI.UpdateUI();
        }
    }

    public void OpenUI() {
        Show();

        _targetIndicatorObj.SetActive(true);
    }

    public void HideUI() {
        Hide();

        _targetIndicatorObj.SetActive(false);
    }

    public void ShowTargetUI(CharacterStats characterStats) {
        _targetUI.OpenUI(characterStats);
    }

    public void HideTargetUI() {
        _targetUI.Hide();
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
