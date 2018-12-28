using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage all UI stuff in the game.
/// </summary>
public class UIManager : MonoBehaviour {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static UIManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    /// <summary>
    /// LoginManager reference to manage loginPanel UIs.
    /// See <see cref="LoginManager"/>
    /// </summary>
    public LoginManager loginManager;

    /// <summary>
    /// ClassManager reference to manage selectClassPanel UIs.
    /// See <see cref="ClassManager"/>
    /// </summary>
    public ClassManager classManager;

    /// <summary>
    /// Hides login panel.
    /// </summary>
    public void HideLoginPanel() {
        loginManager.Hide();
    }

    /// <summary>
    /// Shows login panel.
    /// </summary>
    public void ShowLoginPanel() {
        loginManager.Show();
    }

    /// <summary>
    /// Hides selectClass panel.
    /// </summary>
    public void HideSelectClassPanel() {
        classManager.Hide();
    }

    /// <summary>
    /// Shows selectClass panel.
    /// </summary>
    public void ShowSelectClassPanel() {
        classManager.Show();
    }

}
