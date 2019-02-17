using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : Menu {

    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtLevel;
    [SerializeField]
    private Image _imgHealthBar;
    [SerializeField]
    private Image _imgManaBar;
    [SerializeField]
    private Image _imgTargetIndicatorCircle;
    [SerializeField]
    private DamageIndicatorController _damageIndicatorController;

    private CharacterController _characterController;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();

        _characterController.onInitialized += Initialize;
        _characterController.onTakeDamage += OnTakeDamage;
        _characterController.onHealthRegenerated += OnHealthRegenerated;
    }

    public void OpenUI() {
        Show();
    }

    public void HideUI() {
        Hide();
    }

    private void UpdateUI() {
        SetName();
        SetLevel();
        SetHealthBar();
        SetManaBar();
    }

    private void Initialize() {
        UpdateUI();
    }

    private void OnHealthRegenerated(int healthAmount) {
        UpdateUI();

        _damageIndicatorController.CreateDamageIndicator(DamageIndicatorController.Type.Heal, healthAmount.ToString(), this.transform);
    }

    private void OnTakeDamage(int damage) {
        UpdateUI();

        _damageIndicatorController.CreateDamageIndicator(DamageIndicatorController.Type.Damage, damage.ToString(), this.transform);
    }

    private void SetName() {
        _txtName.text = _characterController.CharacterStats.GetName();
    }

    private void SetLevel() {
        _txtLevel.text = _characterController.CharacterStats.GetLevel().ToString();
    }

    private void SetHealthBar() {
        _imgHealthBar.fillAmount = ((float)_characterController.CharacterStats.GetCurrentHealth() / (float)_characterController.CharacterStats.GetMaxHealth());

        if (_imgHealthBar.fillAmount == 0) {
            _imgHealthBar.color = new Color(_imgHealthBar.color.r, _imgHealthBar.color.g, _imgHealthBar.color.b, 110);
        } else {
            _imgHealthBar.color = new Color(_imgHealthBar.color.r, _imgHealthBar.color.g, _imgHealthBar.color.b, 255);
        }
    }

    private void SetManaBar() {
        _imgManaBar.fillAmount = (_characterController.CharacterStats.GetCurrentMana() / _characterController.CharacterStats.GetMaxMana());
    }
}
