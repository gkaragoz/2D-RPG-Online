﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShiftServer.Client;

public class LoginManager : Menu
{

    #region Singleton

    public static LoginManager instance;
 

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
            NetworkManager.client.AddEventListener(MSServerEvent.MsJoinRequestSuccess, this.OnJoinSuccess);
            NetworkManager.client.AddEventListener(MSPlayerEvent.MsOuse, this.OnPlayerObjectUse);
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
        NetworkManager.client.AddEventListener(MSServerEvent.MsWorldUpdate, NetworkManager.instance.OnWorldUpdate);

        Debug.Log("OnJoinSuccess::EVENT::FIRED");
        //gameObject.SetActive(false);
        UIManager.instance.HideSelectClassPanel();

        ShiftServerData newData = new ShiftServerData();
        newData.Interaction = new ObjectAction();
        newData.Interaction.CurrentObject = new sGameObject
        {
            PosX = 0,
        };

        NetworkManager.client.SendMessage(MSPlayerEvent.MsOuse, newData);
    }

    public void OnPlayerObjectUse(ShiftServerData data)
    {
        Debug.Log("OnPlayerObjectUse event triggered");
    }
    public void SendJoinPacket()
    {
        ShiftServerData data = new ShiftServerData();
        data.ClData = NetworkManager.clientinfo;
        NetworkManager.client.SendMessage(MSServerEvent.MsJoinRequest, data);
    }
}
