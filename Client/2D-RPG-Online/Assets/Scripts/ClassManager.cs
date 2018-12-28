using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : Menu {

    #region Singleton

    public static ClassManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

}
