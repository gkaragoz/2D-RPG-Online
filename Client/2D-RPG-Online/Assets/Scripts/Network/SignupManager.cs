using CI.HttpClient;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public class SignupData {
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

    public void Signup() {
        if (IsUsernameValid && IsPasswordValid && IsEmailValid && IsURLEmpty) {
            LogManager.instance.AddLog(SIGNUP, Log.Type.Server);

            HttpClient client = new HttpClient();

            SignupData data = new SignupData();
            data.username = _inputFieldUsername.text;
            data.password = _inputFieldPassword.text;
            data.email = _inputFieldEmail.text;

            string jsonData = JsonUtility.ToJson(data);

            IHttpContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            client.Post(new Uri(URL), content, HttpCompletionOption.AllResponseContent, response => {
                Debug.Log(response);
            });
        } else {
            LogManager.instance.AddLog("You must fill input fields!", Log.Type.Error);
        }
    }

    public void GoToLoginPage() {
        this.Hide();
        LoginManager.instance.Show();
    }

}
