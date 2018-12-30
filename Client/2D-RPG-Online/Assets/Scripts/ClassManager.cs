using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for collect user inputs about selected class.
/// </summary>
public class ClassManager : Menu {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static ClassManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

}
