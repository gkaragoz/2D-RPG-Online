using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is repsonsible to manage Login, Login as a guest or Join the game events.
/// </summary>
public class LoginManager : Menu {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static LoginManager instance;

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
    /// Login into the game.
    /// </summary>
    /// <remarks>
    /// <para>Checks and gets username & password input fields.</para>
    /// <para>Sends a request to login with username and password.</para>
    /// </remarks>
    public void Login() {
        //TODO Checks and gets username & password input fields.
        //TODO Sends a request to login with username and password.
    }

    /// <summary>
    /// Login as a guest into the game.
    /// </summary>
    /// <remarks>
    /// <para>Sends a request to login as a guest.</para>
    /// </remarks>
    public void LoginAsAGuest() {
        UIManager.instance.HideLoginPanel();
        UIManager.instance.ShowSelectClassPanel();

        //TODO Sends a request to login as a guest.
    }

    /// <summary>
    /// Joining to a random game.
    /// </summary>
    /// <remarks>
    /// <para>Sends a request to join a game.</para>
    /// </remarks>
    public void JoinGame() {
        UIManager.instance.HideSelectClassPanel();

        //TODO Send a request to join game.
    }

}
