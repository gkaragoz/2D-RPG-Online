using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupMessage : Menu {

    public enum Type {
        Info,
        Error,
        Success
    }

    [System.Serializable]
    public class Theme {
        public GameObject iconInfo;
        public GameObject iconSuccess;
        public GameObject iconError;
        public Color defaultColor;
        public Color successColor;
        public Color errorColor;
    }

    [SerializeField]
    private TextMeshProUGUI _txtTitle;
    [SerializeField]
    private TextMeshProUGUI _txtMessage;

    [Header("Settings")]
    [SerializeField]
    private Theme _theme;

    public void SetMessage(string text) {
        _txtMessage.text = text;
    }

    public void Show(string title, string message, Type type) {
        _txtTitle.text = title;

        switch (type) {
            case Type.Info:
                SetDefaultUI();
                break;
            case Type.Error:
                SetErrorUI();
                break;
            case Type.Success:
                SetSuccessUI();
                break;
            default:
                SetDefaultUI();
                break;
        }

        base.Show();

        SetMessage(message);
    }

    public override void Hide() {
        base.Hide();
    }

    private void SetSuccessUI() {
        _theme.iconInfo.SetActive(false);
        _theme.iconError.SetActive(false);
        _theme.iconSuccess.SetActive(true);

        _txtMessage.color = _theme.successColor;
    }

    private void SetDefaultUI() {
        _theme.iconInfo.SetActive(true);
        _theme.iconSuccess.SetActive(false);
        _theme.iconError.SetActive(false);

        _txtMessage.color = _theme.defaultColor;
    }

    private void SetErrorUI() {
        _theme.iconInfo.SetActive(false);
        _theme.iconSuccess.SetActive(false);
        _theme.iconError.SetActive(true);

        _txtMessage.color = _theme.errorColor;
    }

}
