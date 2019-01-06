using UnityEngine;
using ShiftServer.Client;
using ShiftServer.Client.Data.Entities;
using System;

public class NetworkManager : MonoBehaviour {
    
    public static ManaShiftServer mss;

    [SerializeField]
    private string _hostName = "127.0.0.0";

    [SerializeField]
    private int _port = 1337;

    private ConfigData _cfg;

    private const string CONNECT = "Trying connect to the server... ";
    private const string ON_CONNECTION_SUCCESS = "Connection success!";
    private const string ON_CONNECTION_FAILED = "Connection failed!";
    private const string ON_CONNECTION_LOST = "Connection lost!";

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static NetworkManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);

        InitNetwork();
    }

    private void InitNetwork() {
        mss = new ManaShiftServer();
        mss.AddEventListener(MSServerEvent.Connection, OnConnectionSuccess);
        mss.AddEventListener(MSServerEvent.ConnectionFailed, OnConnectionFailed);
        mss.AddEventListener(MSServerEvent.ConnectionLost, OnConnectionLost);

        _cfg = new ConfigData();
        _cfg.Host = _hostName;
        _cfg.Port = _port;

        //LogManager.instance.AddLog(CONNECT + _cfg.Host + ":" + _cfg.Port, Log.Type.Server);
        mss.Connect(_cfg);
    }

    private void Update() {
        if (mss != null) {
            mss.Listen();
        }
    }

    private void OnConnectionSuccess(ShiftServerData data) {
        LogManager.instance.AddLog(ON_CONNECTION_SUCCESS, Log.Type.Server);
    }

    private void OnConnectionFailed(ShiftServerData data) {
        LogManager.instance.AddLog(ON_CONNECTION_FAILED, Log.Type.Server);
    }

    private void OnConnectionLost(ShiftServerData data) {
        LogManager.instance.AddLog(ON_CONNECTION_LOST, Log.Type.Server);
    }

    private void OnApplicationQuit() {
        if (mss.IsConnected) {
            mss.Disconnect();
        }
    }
}

