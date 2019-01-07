using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : Menu {

    [System.Serializable]
    public class Theme {
        public GameObject iconSuccess;
        public GameObject iconError;
        public Color defaultColor;
        public Color successColor;
        public Color errorColor;
    }

    [SerializeField]
    private GameObject _loadingBarContiner;
    [SerializeField]
    private Image _loadingBar;
    [SerializeField]
    private TextMeshProUGUI _txtTitle;
    [SerializeField]
    private TextMeshProUGUI _txtMessage;

    [Header("Settings")]
    [SerializeField]
    private Theme _theme;
    [SerializeField]
    private float _speed;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private float _currentAmount;

    public void SetMessage(string text) {
        _txtMessage.text = text;
    }

    public void Show(string message) {
        _txtTitle.text = "Please Wait!";

        SetDefaultUI();

        base.Show();

        SetMessage(message);
    }

    public void HideAfter(float seconds, bool error = false) {
        _txtTitle.text = "Result!";

        if (error) {
            SetErrorUI();
        } else {
            SetSuccessUI();
        }

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

    private void SetSuccessUI() {
        _loadingBarContiner.SetActive(false);
        _theme.iconError.SetActive(false);
        _theme.iconSuccess.SetActive(true);

        _txtMessage.color = _theme.successColor;
    }

    private void SetDefaultUI() {
        _theme.iconSuccess.SetActive(false);
        _theme.iconError.SetActive(false);
        _loadingBarContiner.SetActive(true);

        _txtMessage.color = _theme.defaultColor;
    }

    private void SetErrorUI() {
        _loadingBarContiner.SetActive(false);
        _theme.iconSuccess.SetActive(false);
        _theme.iconError.SetActive(true);

        _txtMessage.color = _theme.errorColor;
    }

}
