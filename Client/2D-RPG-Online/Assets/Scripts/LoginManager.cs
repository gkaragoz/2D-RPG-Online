using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShiftServer.Client;
using System;

public class LoginManager : Menu
{

    #region Singleton

    public static LoginManager instance;
    public ClientData clientInfo;
 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion
    private void Start()
    {
        if (!NetworkManager.instance.IsOffline)
        {
            NetworkManager.client.AddEventListener(MSServerEvent.RoomJoin, this.OnJoinSuccess);
            NetworkManager.client.AddEventListener(MSPlayerEvent.CreatePlayer, this.OnPlayerCreate);

            clientInfo = new ClientData();
            clientInfo.Loginname = "Test";
            clientInfo.MachineId = SystemInfo.deviceUniqueIdentifier;
            clientInfo.MachineName = SystemInfo.deviceName;

        }

    }
    public void Login()
    {
        //Check and get username & password input fields.

        //Send a request to login with username and password. 

    }

    public void LoginAsAGuest()
    {
        UIManager.instance.HideLoginPanel();
        UIManager.instance.ShowSelectClassPanel();

        //Send a request to login as a guest.

        //TO-DO: SEND REQUEST TO AUTH SERVER AND GET TOKEN IF SUCCESS
    }

    public void JoinGame()
    {

        //Send a request to join game.    
        if (NetworkManager.instance.IsOffline)
        {
            UIManager.instance.HideSelectClassPanel();
        }
        else
        {
            this.SendJoinPacket();
        }
    }

    public void OnJoinSuccess(ShiftServerData data)
    {
        NetworkManager.client.AddEventListener(MSPlayerEvent.WorldUpdate, NetworkManager.instance.OnWorldUpdate);

        Debug.Log("OnJoinSuccess::EVENT::FIRED");
        //gameObject.SetActive(false);
        UIManager.instance.HideSelectClassPanel();


        ShiftServerData newData = new ShiftServerData();
        newData.Session = data.Session;
        
      
        newData.ClientInfo = clientInfo;

        NetworkManager.client.SendMessage(MSPlayerEvent.CreatePlayer, newData);
    }

    public void OnPlayerCreate(ShiftServerData data)
    {


        Debug.Log("OnPlayerCreate event triggered");
    }
    public void SendJoinPacket()
    {
        ShiftServerData data = new ShiftServerData();
        data.ClientInfo = this.clientInfo;
        NetworkManager.client.SendMessage(MSServerEvent.RoomJoin, data);
    }
}
