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
    public int pingRequestedTick;


    private void Start()
    {
        client = new NetworkClient();
        client.AddEventListener(MSServerEvent.MsConnectOk, this.OnConnected);
        client.AddEventListener(MSServerEvent.MsPingRequest, OnPingResponse);

        client.Connect("localhost", 1337);

        clientinfo = new ClientData();
        clientinfo.Guid = Guid.NewGuid().ToString();
        clientinfo.Loginname = "Test";
        clientinfo.MachineId = SystemInfo.deviceUniqueIdentifier;
        clientinfo.MachineName = SystemInfo.deviceName;

        StartCoroutine(Listener());

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
                pingRequestedTick = DateTime.UtcNow.Millisecond;
                client.SendMessage(MSServerEvent.MsPingRequest, null);
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
        int pongTick = DateTime.UtcNow.Millisecond;
        Debug.Log("Ping: " + (pongTick - pingRequestedTick) + " ms");
    }
    public void OnConnected(ShiftServerData data)
    {
        StartCoroutine(SendPing());
        Debug.Log("Connected To Server");
    }

    void OnApplicationQuit()
    {
        // Always disconnect before quitting
        if (client.IsConnected())
            client.Disconnect();
    }
}

