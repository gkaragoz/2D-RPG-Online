using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : Menu {

    [SerializeField]
    private Image _loadingBar;
    [SerializeField]
    private TextMeshProUGUI _txtMessage;
    [SerializeField]
    private float _speed;
    [SerializeField]
    [Utils.ReadOnly]
    private float _currentAmount;

    public void SetMessage(string text) {
        _txtMessage.text = text;
    }

    public void Show(string message) {
        base.Show();

        SetMessage(message);
    }

    public void HideAfter(float seconds) {
        Invoke("Hide", seconds);
    }

    private void Update() {
        if (_currentAmount < 100) {
            _currentAmount += _speed * Time.deltaTime;
        } else {
            _currentAmount = 0;
        }

        _loadingBar.fillAmount = _currentAmount / 100;
    }

}
