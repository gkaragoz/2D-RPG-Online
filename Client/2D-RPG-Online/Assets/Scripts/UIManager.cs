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

    public LoginManager loginManager;
    public ClassManager classManager;

    public void HideLoginPanel() {
        loginManager.Hide();
    }

    public void ShowLoginPanel() {
        loginManager.Show();
    }

    public void HideSelectClassPanel() {
        classManager.Hide();
    }

    public void ShowSelectClassPanel() {
        classManager.Show();
    }

}
