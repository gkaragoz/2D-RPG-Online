using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void ShowLoadingPopup() {
        _popupLoading.Show();
    }

    public void HideLoadingPopup() {
        _popupLoading.Hide();
    }

}
