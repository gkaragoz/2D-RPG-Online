﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ShiftServer.Client;

public class NetworkManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static NetworkManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion
    public static NetworkClient client;

    [SerializeField]
    private string _hostName = "192.168.1.2";

    [SerializeField]
    private int _port = 1337;

    [SerializeField]
    private bool _isOffline = false;

    public bool IsOffline
    {
        get
        {
            return _isOffline;
        }

        private set
        {
            _isOffline = value;
        }
    }

    private void Start()
    {
        if (!IsOffline)
        {
            client = new NetworkClient();
            client.AddEventListener(MSServerEvent.ConnectOk, this.OnConnected);
            client.AddEventListener(MSServerEvent.PingRequest, OnPingResponse);

            client.Connect(_hostName, _port);

            StartCoroutine(Listener());
        }
    }

    private void Update()
    {
        //client.Listen();
    }

    public IEnumerator SendPing()
    {
        while (true)
        {
            if (client.IsConnected())
            {
                ShiftServerData data = new ShiftServerData();
                client.SendMessage(MSServerEvent.PingRequest, null);
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                StopCoroutine(SendPing());
            }
          
        }
    }

    private IEnumerator Listener()
    {
        while (true)
        {
            client.Listen();
            yield return new WaitForSeconds(1f);
        }
          
    }
    private void OnPingResponse(ShiftServerData obj)
    {

    }

    public void OnWorldUpdate(ShiftServerData data)
    {
        Debug.Log("!! WORLD UPDATED !!");
    }

    public void OnConnected(ShiftServerData data)
    {
        StartCoroutine(SendPing());
        Debug.Log("Connected To Server");
    }

    void OnApplicationQuit()
    {
        if (!IsOffline)
        {
            // Always disconnect before quitting
            if (client.IsConnected())
                client.Disconnect();

            StopAllCoroutines();

        }
    }
}

