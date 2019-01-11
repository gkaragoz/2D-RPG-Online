using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Singleton

    public static GameManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    private void Start() {
        List<Task> tasks = new List<Task>();
        tasks.Add(GooglePlayManager.instance.initializationProgress);
        tasks.Add(GooglePlayManager.instance.signInResponseProgress);
        tasks.Add(GooglePlayManager.instance.sessionIdResponseProgress);
        tasks.Add(GooglePlayManager.instance.accountDataResponseProgress);

        LoadingManager.instance.SetCheckList(tasks);

        GooglePlayManager.instance.Initialize();
        GooglePlayManager.instance.SignIn();
    }

}
