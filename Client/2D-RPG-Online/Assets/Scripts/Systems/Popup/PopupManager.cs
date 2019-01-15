using UnityEngine;

public class PopupManager : MonoBehaviour {

    #region Singleton

    public static PopupManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [SerializeField]
    private PopupLoading _popupLoading;
    [SerializeField]
    private PopupMessage _popupMessage;

    public void ShowPopupLoading(string message) {
        _popupLoading.Show(message);
    }

    public void HidePopupLoading(string message, float seconds = 0f, bool error = false) {
        _popupLoading.SetMessage(message);
        _popupLoading.HideAfter(seconds);
    }

    public void ShowPopupMessage(string title, string message, PopupMessage.Type popupType) {
        _popupMessage.Show(title, message, popupType);
    }

    public void HidePopupMessage() {
        _popupMessage.Hide();
    }

}
