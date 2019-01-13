using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Menu {

    #region Singleton

    public static MenuManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Task initializationProgress;

    public void Initialize() {

        initializationProgress?.Invoke();
    }

}
