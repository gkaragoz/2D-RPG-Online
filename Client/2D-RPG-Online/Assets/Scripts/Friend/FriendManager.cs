﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendManager : Menu {

    #region Singleton

    public static FriendManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public void Initialize() {

    }

}
