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

    public void Signup() {
        HttpClient client = new HttpClient();

        SignupData data = new SignupData();
        data.username = _inputFieldUsername.text;
        data.password = _inputFieldPassword.text;
        data.email = _inputFieldEmail.text;

        string jsonData = JsonUtility.ToJson(data);

        IHttpContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        client.Post(new Uri(URL), content, HttpCompletionOption.AllResponseContent, response =>
        {
            Debug.Log(response);
        });
    }

    public void GoToLoginPage() {
        this.Hide();
        LoginManager.instance.Show();
    }

}
