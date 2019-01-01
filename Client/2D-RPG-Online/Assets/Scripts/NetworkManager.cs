using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ShiftServer.Client;
using ShiftServer.Client.Data.Entities;

public class NetworkManager : MonoBehaviour {
    #region Singleton

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
    }

    #endregion

    public static ManaShiftServer mss;

    [SerializeField]
    private string _hostName = "192.168.1.2";

    [SerializeField]
    private int _port = 1337;

    [SerializeField]
    private bool _isOffline = false;

    public bool IsOffline {
        get {
            return _isOffline;
        }

        private set {
            _isOffline = value;
        }
    }

    private void Start() {
        if (!IsOffline) {
            mss = new ManaShiftServer();
            //mss.AddEventListener(MSServerEvent.Connection, this.OnConnected);
            //mss.AddEventListener(MSServerEvent.PingRequest, OnPingResponse);

            ConfigData cfg = new ConfigData();
            cfg.Host = _hostName;
            cfg.Port = _port;

            mss.Connect(cfg);
        }
    }

    private void Update() {
        //client.Listen();
    }

    private void OnApplicationQuit() {
        if (!IsOffline) {
            // Always disconnect before quitting
            if (mss.IsConnected()) {
                mss.Disconnect();
            }
        }
    }
}

