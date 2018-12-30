using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ShiftServer.Client;

public class NetworkManager : MonoBehaviour
{
    
    public static NetworkClient client;
    public static ClientData clientinfo;



    private void Start()
    {
        client = new NetworkClient();
        client.Connect("localhost", 1337);

        clientinfo = new ClientData();
        clientinfo.Guid = Guid.NewGuid().ToString();
        clientinfo.Loginname = "Test";
        clientinfo.MachineId = SystemInfo.deviceUniqueIdentifier;
        clientinfo.MachineName = SystemInfo.deviceName;

    }
    private void Update()
    {
        client.Listen();
    }
    void OnApplicationQuit()
    {
        // Always disconnect before quitting
        if (client.IsConnected())
            client.Disconnect();
    }
}

