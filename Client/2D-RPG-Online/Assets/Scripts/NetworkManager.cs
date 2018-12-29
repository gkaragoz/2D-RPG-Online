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

    private void OnConnected(ShiftServerMsg obj)
    {
        Debug.Log("Connected To Server");
    }

    IEnumerator foo()
    {

        yield return new WaitForEndOfFrame();

    }

 
    private void FixedUpdate()
    {
        
    }

    public static void SendMessage(ShiftServerMsgID msgId)
    {
        networkClient.SendMessage(msgId);
    }


}

