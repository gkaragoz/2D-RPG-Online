using UnityEngine;
using ShiftServer.Client;
using ShiftServer.Client.Data.Entities;
using System;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour {

    public Func<bool> onGameplayServerConnectionSuccess;

    public static ManaShiftServer mss;
    public static string SessionID { get; set; }

    public static string UserID {
        get {
            return PlayerPrefs.GetString(USER_ID);
        }
        set {
            PlayerPrefs.SetString(USER_ID, value);
        }
    }

    public bool Reconciliaton { get { return _reconciliaton; } }

    [SerializeField]
    private string _hostName = "127.0.0.0";

    [SerializeField]
    private int _port = 1337;

    [SerializeField]
    private bool _reconciliaton;

    private ConfigData _cfg;

    private const string USER_ID = "USER_ID";

    private const string CONNECT = "Trying connect to the server... ";
    private const string ON_CONNECTION_SUCCESS = "Connection success!";
    private const string ON_CONNECTION_FAILED = "Connection failed!";
    private const string ON_CONNECTION_LOST = "Connection lost!";

    public static NetworkManager instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);

        InitializeGameplayServer();
    }

    public void ConnectToGameplayServer() {
        Debug.Log(CONNECT);

        _cfg = new ConfigData();
        _cfg.Host = _hostName;
        _cfg.Port = _port;
        _cfg.SessionID = SessionID;

        StartCoroutine(mss.Connect(_cfg));
        PopupManager.instance.ShowPopupLoading("Connecting to server...");
    }

    private void InitializeGameplayServer() {
        mss = new ManaShiftServer();
        mss.AddEventListener(MSServerEvent.Connection, OnConnectionSuccess);
        mss.AddEventListener(MSServerEvent.ConnectionFailed, OnConnectionFailed);
        mss.AddEventListener(MSServerEvent.ConnectionLost, OnConnectionLost);
    }

    private void OnConnectionSuccess(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_SUCCESS + data);

        onGameplayServerConnectionSuccess?.Invoke();
    }

    private void OnConnectionFailed(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_FAILED + data);
    }

    private void OnConnectionLost(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_LOST + data);
    }

    private void OnApplicationQuit() {
        if (mss != null) {
            if (mss.IsConnected) {
                mss.Disconnect();
            }
        }
    }
}

