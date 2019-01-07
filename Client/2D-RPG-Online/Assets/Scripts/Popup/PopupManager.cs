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

    public void ShowLoadingPopup(string message) {
        _popupLoading.Show(message);
    }

    public void HideLoadingPopup(string message, float seconds = 0f) {
        _popupLoading.SetMessage(message);
        _popupLoading.HideAfter(seconds);
    }

}
