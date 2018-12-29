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
    
    private static NetworkClient networkClient;



    private void Start()
    {
        networkClient = new NetworkClient();
        networkClient.Connect("localhost", 1337);
        networkClient.AddEventListener(ShiftServerMsgID.ShiftServerConnectOk, OnConnected);

    }

    private void OnDestroy()
    {
        if (networkClient != null)
            networkClient.Disconnect();
    }

    private void OnConnected(ShiftServerMsg obj)
    {
        Debug.Log("Connected To Server");
    }

 
    public static void SendMessage(ShiftServerMsg data)
    {
        networkClient.SendMessage(data);
    }


}

