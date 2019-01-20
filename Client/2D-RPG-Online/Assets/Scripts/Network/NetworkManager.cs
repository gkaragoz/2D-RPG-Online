using UnityEngine;
using ShiftServer.Client;
using ShiftServer.Client.Data.Entities;
using System;
using System.Threading.Tasks;
using System.Collections;

public class NetworkManager : MonoBehaviour {

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

    private bool _hasConnectionProblem = false;

    public static NetworkManager instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);

        InitializeGameplayServer();
    }

    public IEnumerator ConnectToGameplayServer(Action<bool> success) {
        Debug.Log(CONNECT);

        _cfg = new ConfigData();
        _cfg.Host = _hostName;
        _cfg.Port = _port;
        _cfg.SessionID = SessionID;

        mss.Connect(_cfg, 3);

        while (!mss.IsConnected) {
            yield return new WaitForSeconds(0.2f);

            if (_hasConnectionProblem) {
                success(false);

                StopAllCoroutines();
                break;
            }
        }

        success(true);
    }

    private void InitializeGameplayServer() {
        mss = new ManaShiftServer();
        mss.AddEventListener(MSServerEvent.Connection, OnConnectionSuccess);
        mss.AddEventListener(MSServerEvent.ConnectionFailed, OnConnectionFailed);
        mss.AddEventListener(MSServerEvent.ConnectionLost, OnConnectionLost);
    }

    private void OnConnectionSuccess(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_SUCCESS + data);

        _hasConnectionProblem = false;
    }

    private void OnConnectionFailed(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_FAILED + data);

        _hasConnectionProblem = true;
    }

    private void OnConnectionLost(ShiftServerData data) {
        Debug.Log(ON_CONNECTION_LOST + data);

        _hasConnectionProblem = true;
    }

    private void OnApplicationQuit() {
        if (mss != null) {
            if (mss.IsConnected) {
                mss.Disconnect();
            }
        }
    }
}

