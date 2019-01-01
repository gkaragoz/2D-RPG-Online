using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : Menu {

    #region Singleton

    public static LoginManager instance;
    public ClientData clientInfo;


    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [System.Serializable]
    public class UISettings {
        public TMP_InputField inputFieldUsername;
        public TMP_InputField inputFieldPassword;

        public bool IsUsernameValid {
            get { return inputFieldUsername.text != string.Empty ? true : false; }
        }

        public bool IsPasswordValid {
            get { return inputFieldPassword.text != string.Empty ? true : false; }
        }

        public string GetUsername() {
            return inputFieldUsername.text;
        }

        public string GetPassword() {
            return inputFieldPassword.text;
        }

        public void ActivateInputFields() {
            inputFieldUsername.interactable = true;
            inputFieldPassword.interactable = true;
        }

        public void DisableInputFields() {
            inputFieldUsername.interactable = false;
            inputFieldPassword.interactable = false;
        }
    }

    [Header("Initialization")]
    [SerializeField]
    private UISettings _UISettings;

    private const string JOIN = "Trying to join into the account... ";
    private const string ON_LOGIN_SUCCESS = "Login success!";
    private const string ON_LOGIN_FAILED = "Login failed!";

    private void Start() {
        if (!NetworkManager.instance.OfflineMode) {
            NetworkManager.mss.AddEventListener(MSServerEvent.Login, OnLoginSuccess);
            NetworkManager.mss.AddEventListener(MSServerEvent.LoginFailed, OnLoginFailed);
        }
    }

    private void Update() {
        if (NetworkManager.mss.IsConnected()) {
            _UISettings.ActivateInputFields();
        } else {
            _UISettings.DisableInputFields();
        }
    }

    public void Login() {
        if (_UISettings.IsUsernameValid && _UISettings.IsPasswordValid) {
            LogManager.instance.AddLog(JOIN, Log.Type.Server);

            Account account = new Account();
            account.Username = _UISettings.GetUsername();
            account.Password = _UISettings.GetPassword();

            clientInfo = new ClientData();
            clientInfo.Loginname = "Test";
            clientInfo.MachineId = SystemInfo.deviceUniqueIdentifier;
            clientInfo.MachineName = SystemInfo.deviceName;
            clientInfo.Ver = "0.1";

            ShiftServerData data = new ShiftServerData();
            data.Account = account;
            data.ClientInfo = clientInfo;

            NetworkManager.mss.SendMessage(MSServerEvent.Login, data);
        } else {
            LogManager.instance.AddLog("You must fill those input fields!", Log.Type.Error);
        }
    }

    private void OnLoginSuccess(ShiftServerData data) {
        LogManager.instance.AddLog(ON_LOGIN_SUCCESS, Log.Type.Server);

        LogManager.instance.AddLog("Welcome " + data.AccountData.Username + "!", Log.Type.Server);
        LogManager.instance.AddLog("Your virtual money is " + data.AccountData.VirtualMoney, Log.Type.Server);
        LogManager.instance.AddLog("Your special virtual money is " + data.AccountData.VirtualSpecialMoney, Log.Type.Server);

        LogManager.instance.AddLog("Active rooms count on server is " + data.RoomData.Rooms.Count, Log.Type.Server);

        LogManager.instance.AddLog("Your session ID is " + data.Session.Sid, Log.Type.Server);

        LogManager.instance.AddLog("Press TAB to toggle Log Panel.", Log.Type.Info);
        LogManager.instance.AddLog("Press 1 to show Info message.", Log.Type.Info);
        LogManager.instance.AddLog("Press 2 to show Error message.", Log.Type.Error);
        LogManager.instance.AddLog("Press 3 to show Loot message.", Log.Type.Loot);
        LogManager.instance.AddLog("Press 4 to show Interact message.", Log.Type.Interact);
        LogManager.instance.AddLog("Press 5 to show Drop message.", Log.Type.Drop);
    }

    private void OnLoginFailed(ShiftServerData data) {
        LogManager.instance.AddLog(ON_LOGIN_FAILED, Log.Type.Server);
    }

}
