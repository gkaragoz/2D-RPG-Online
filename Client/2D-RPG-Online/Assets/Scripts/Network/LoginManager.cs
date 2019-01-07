using ShiftServer.Proto.Models;
using SimpleHTTP;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public class LoginData {
        public string username;
        public string password;
    }

    [SerializeField]
    private TMP_InputField _inputFieldUsername;
    [SerializeField]
    private TMP_InputField _inputFieldPassword;
    [SerializeField]
    private Button _btnLogin;
    [SerializeField]
    private Button _btnSignup;

    [Header("Settings")]
    public string URL;

    public bool IsURLEmpty {
        get { return URL == string.Empty ? true : false; }
    }

    public bool IsUsernameValid {
        get { return _inputFieldUsername.text != string.Empty ? true : false; }
    }

    public bool IsPasswordValid {
        get { return _inputFieldPassword.text != string.Empty ? true : false; }
    }

    public string GetUsername() {
        return _inputFieldUsername.text;
    }

    public string GetPassword() {
        return _inputFieldPassword.text;
    }

    private const string LOGIN = "Trying to login into the account... ";
    private const string ON_LOGIN_SUCCESS = "Login success!";
    private const string ON_LOGIN_FAILED = "Login failed!";

    public void Login() {
        if (IsUsernameValid && IsPasswordValid && !IsURLEmpty) {
            LogManager.instance.AddLog(LOGIN, Log.Type.Server);

            StartCoroutine(ILoginPostMethod());
        } else {
            LogManager.instance.AddLog("You must fill input fields!", Log.Type.Error);
        }
    }

    private IEnumerator ILoginPostMethod() {
        LoginData data = new LoginData();
        data.username = _inputFieldUsername.text;
        data.password = _inputFieldPassword.text;

        JsonUtility.ToJson(data);

        Request request = new Request(URL)
            .Post(RequestBody.From<LoginData>(data));

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            LogManager.instance.AddLog(ON_LOGIN_SUCCESS, Log.Type.Server);

            this.Hide();
            LobbyManager.instance.Initialize();
            RoomManager.instance.Initialize();
            FriendManager.instance.Initialize();
            LobbyManager.instance.Show();
            FriendManager.instance.Show();

            //LogManager.instance.AddLog("Welcome " + data.AccountData.Username + "!", Log.Type.Server);
            //LogManager.instance.AddLog("Your virtual money is " + data.AccountData.VirtualMoney, Log.Type.Server);
            //LogManager.instance.AddLog("Your special virtual money is " + data.AccountData.VirtualSpecialMoney, Log.Type.Server);
            //LogManager.instance.AddLog("Your session ID is " + data.Session.Sid, Log.Type.Server);
        } else {
            Debug.Log("error: " + http.Error());
            LogManager.instance.AddLog(ON_LOGIN_FAILED, Log.Type.Server);
        }
    }

    public void GoToSignupPage() {
        this.Hide();
        SignupManager.instance.Show();
    }

    public void ActivateLoginUIs() {
        _inputFieldUsername.interactable = true;
        _inputFieldPassword.interactable = true;
        _btnLogin.interactable = true;
    }

    public void DeactivateLoginUIS() {
        _inputFieldUsername.interactable = false;
        _inputFieldPassword.interactable = false;
        _btnLogin.interactable = false;
    }

}
