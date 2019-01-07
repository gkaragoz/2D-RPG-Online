﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ShiftServer.Proto.Models;
using System.Collections;
using SimpleHTTP;

public class SignupManager : Menu {

    #region Singleton

    public static SignupManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public class SignUpData {
        public string username;
        public string password;
        public string email;
    }

    [SerializeField]
    private TMP_InputField _inputFieldUsername;
    [SerializeField]
    private TMP_InputField _inputFieldPassword;
    [SerializeField]
    private TMP_InputField _inputFieldEmail;
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

    public bool IsEmailValid {
        get { return _inputFieldEmail.text != string.Empty ? true : false; }
    }

    public string GetUsername() {
        return _inputFieldUsername.text;
    }

    public string GetPassword() {
        return _inputFieldPassword.text;
    }

    private const string SIGNUP = "Trying to create a new account... ";
    private const string ON_SIGNUP_SUCCESS = "Sign up success!";
    private const string ON_SIGNUP_FAILED = "Sign up failed!";
    private const string ON_CAN_NOT_CONNECT_TO_HOST = "Signup failed! Can not connect to host!";

    public void Signup() {
        if (IsUsernameValid && IsPasswordValid && IsEmailValid && !IsURLEmpty) {
            LogManager.instance.AddLog(SIGNUP, Log.Type.Server);

            StartCoroutine(ISignupPostMethod());
        } else {
            LogManager.instance.AddLog("You must fill input fields!", Log.Type.Error);
        }
    }

    private IEnumerator ISignupPostMethod() {
        PopupManager.instance.ShowLoadingPopup(SIGNUP);

        SignUpData data = new SignUpData();
        data.username = _inputFieldUsername.text;
        data.password = _inputFieldPassword.text;
        data.email = _inputFieldEmail.text;

        JsonUtility.ToJson(data);

        Request request = new Request(URL)
            .Post(RequestBody.From<SignUpData>(data));

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(resp.Body());

            if (authResponse.Success) {
                LogManager.instance.AddLog(ON_SIGNUP_SUCCESS, Log.Type.Server);

                PopupManager.instance.HideLoadingPopup(ON_SIGNUP_SUCCESS, 1f);
            } else {
                LogManager.instance.AddLog(ON_SIGNUP_FAILED, Log.Type.Server);

                PopupManager.instance.HideLoadingPopup(ON_SIGNUP_FAILED, 1f);
            }
        } else {
            Debug.Log("error: " + http.Error());
            PopupManager.instance.HideLoadingPopup(ON_CAN_NOT_CONNECT_TO_HOST, 2f);
        }
    }

    public void GoToLoginPage() {
        this.Hide();
        LoginManager.instance.Show();
    }

}
