using ShiftServer.SocketClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class NetworkManager : Menu
{

    #region Singleton

    public static NetworkManager instance;
    public static NetworkClient networkClient;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        networkClient = new NetworkClient();
        networkClient.Connect("localhost", 1337);
        DontDestroyOnLoad(instance);
    }

    #endregion


}

