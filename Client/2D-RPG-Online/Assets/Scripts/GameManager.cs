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
        GooglePlayManager.instance.onAnyErrorOccured += OnGooglePlayManagerErrorOccured;
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;

        List<Task> tasks = new List<Task>();
        tasks.Add(GooglePlayManager.instance.initializationProgress);
        tasks.Add(GooglePlayManager.instance.signInResponseProgress);
        tasks.Add(GooglePlayManager.instance.sessionIdResponseProgress);
        tasks.Add(GooglePlayManager.instance.accountDataResponseProgress);

        LoadingManager.instance.SetCheckList(tasks);

        GooglePlayManager.instance.Initialize();
        GooglePlayManager.instance.SignIn();
    }

    private void OnGooglePlayManagerErrorOccured(GooglePlayManager.Errors error) {
        switch (error) {
            case GooglePlayManager.Errors.SIGN_IN:
                break;
            case GooglePlayManager.Errors.SIGN_OUT:
                break;
            case GooglePlayManager.Errors.GET_SESSION_ID:
                break;
            case GooglePlayManager.Errors.GET_ACCOUNT_DATA:
                break;
            case GooglePlayManager.Errors.GET_ID_TOKEN:
                break;
            default:
                break;
        }
    }

    private void OnLoadingCompleted() {
        LoadingManager.instance.Hide();
    }
}
