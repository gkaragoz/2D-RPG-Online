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

    [SerializeField]
    private TMP_InputField _inputFieldUsername;
    [SerializeField]
    private TMP_InputField _inputFieldPassword;
    [SerializeField]
    private Button _btnLogin;
    [SerializeField]
    private Button _btnSignup;

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

    private const string JOIN = "Trying to join into the account... ";
    private const string ON_LOGIN_SUCCESS = "Login success!";
    private const string ON_LOGIN_FAILED = "Login failed!";

    private void Start() {
        NetworkManager.mss.AddEventListener(MSServerEvent.Login, OnLoginSuccess);
        NetworkManager.mss.AddEventListener(MSServerEvent.LoginFailed, OnLoginFailed);
    }

    private void Update() {
        if (NetworkManager.mss.IsConnected) {
            ActivateLoginUIs();
        } else {
            DeactivateLoginUIS();
        }
    }

    public void Login() {
        if (IsUsernameValid && IsPasswordValid) {
            LogManager.instance.AddLog(JOIN, Log.Type.Server);

            AccountData account = new AccountData();
            account.Username = GetUsername();
            account.Password = GetPassword();

            ClientData clientInfo = new ClientData();
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

    private void OnLoginSuccess(ShiftServerData data) {
        LogManager.instance.AddLog(ON_LOGIN_SUCCESS, Log.Type.Server);

        this.Hide();
        LobbyManager.instance.Initialize();
        RoomManager.instance.Initialize();
        FriendManager.instance.Initialize();
        LobbyManager.instance.Show();
        FriendManager.instance.Show();

        LogManager.instance.AddLog("Welcome " + data.AccountData.Username + "!", Log.Type.Server);
        LogManager.instance.AddLog("Your virtual money is " + data.AccountData.VirtualMoney, Log.Type.Server);
        LogManager.instance.AddLog("Your special virtual money is " + data.AccountData.VirtualSpecialMoney, Log.Type.Server);

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
