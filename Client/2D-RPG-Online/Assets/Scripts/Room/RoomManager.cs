using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Menu {

    #region Singleton

    public static RoomManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    private Room _joinedRoom;

    public void LeaveRoom() {

    }

}
