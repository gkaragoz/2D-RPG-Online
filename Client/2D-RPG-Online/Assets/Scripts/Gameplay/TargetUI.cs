using UnityEngine;
using UnityEngine.UI;

public class TargetUI : Menu {

    [SerializeField]
    private Image _imgTargetHealthBar;
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtTargetName;

    private CharacterStats _characterStats;

    public void UpdateUI() {
        if (_characterStats != null) {
            SetName();
            SetHealthBar();
        }
    }

    public void OpenUI(CharacterStats characterStats) {
        this._characterStats = characterStats;

        UpdateUI();
        Show();
    }

    public void HideUI() {
        Hide();
    }

    private void SetName() {
        _txtTargetName.text = _characterStats.GetName();
    }

    private void SetHealthBar() {
        _imgTargetHealthBar.fillAmount = ((float)_characterStats.GetCurrentHealth() / (float)_characterStats.GetMaxHealth());
    }

}
