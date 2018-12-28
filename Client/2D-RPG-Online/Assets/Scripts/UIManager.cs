using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    #region Singleton

    public static UIManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public GameObject loginPanel;
    public GameObject selectClassPanel;

    public void HideLoginPanel() {
        loginPanel.SetActive(false);
    }

    public void HideSelectClassPanel() {
        selectClassPanel.SetActive(false);
    }

    public void ShowSelectClassPanel() {
        selectClassPanel.SetActive(true);
    }

}
