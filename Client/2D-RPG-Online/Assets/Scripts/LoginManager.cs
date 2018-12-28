using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : Menu {

    #region Singleton

    public static LoginManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public void Login() {
        //Check and get username & password input fields.

        //Send a request to login with username and password.
    }

    public void LoginAsAGuest() {
        UIManager.instance.HideLoginPanel();
        UIManager.instance.ShowSelectClassPanel();

        //Send a request to login as a guest.
    }

    public void JoinGame() {
        UIManager.instance.HideSelectClassPanel();

        //Send a request to join game.
    }

}
