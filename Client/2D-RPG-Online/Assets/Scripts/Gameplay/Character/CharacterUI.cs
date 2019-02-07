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
    private Image _imgTargetIndicatorCircle;

    private LivingEntity _livingEntity;

    private void Awake() {
        _livingEntity = GetComponent<LivingEntity>();
    }

    public void UpdateUI() {
        SetName();
        SetHealthBar();
        SetManaBar();
    }

    public void OpenUI() {
        Show();
    }

    public void HideUI() {
        Hide();
    }

    private void SetName() {
        _txtName.text = _livingEntity.CharacterStats.GetName();
    }

    private void SetHealthBar() {
        _imgHealthBar.fillAmount = ((float)_livingEntity.CharacterStats.GetCurrentHealth() / (float)_livingEntity.CharacterStats.GetMaxHealth());

        if (_imgHealthBar.fillAmount == 0) {
            _imgHealthBar.transform.parent.gameObject.SetActive(false);
            _imgManaBar.transform.parent.gameObject.SetActive(false);
        }
    }

    private void SetManaBar() {
        _imgManaBar.fillAmount = (_livingEntity.CharacterStats.GetCurrentMana() / _livingEntity.CharacterStats.GetMaxMana());
    }

}
