using ShiftServer.Proto.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Task accountDataResponseProgress;

    private const string ERROR_SIGN_IN_TITLE = "Ups! Google Play!";
    private const string ERROR_SIGN_IN_MESSAGE = "You must logged in your Google account to save your account informations!";

    private const string ERROR_GET_SESSION_ID_TITLE = "Authorization Problem!";
    private const string ERROR_GET_SESSION_ID_MESSAGE = "Some problem occured on server!";

    private const string ERROR_GET_ACCOUNT_DATA_TITLE = "Ups! Account!";
    private const string ERROR_GET_ACCOUNT_DATA_MESSAGE = "Some problem occured on server!";

    private const string ERROR_GET_ID_TOKEN_TITLE = "Wow!";
    private const string ERROR_GET_ID_TOKEN_MESSAGE = "Some problem occured on server!";

    private void Start() {
        Application.targetFrameRate = 60;

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
            case GooglePlayManager.Errors.SIGN_OUT:
                PopupManager.instance.ShowPopupMessage(ERROR_SIGN_IN_TITLE, ERROR_SIGN_IN_MESSAGE, PopupMessage.Type.Info);
                break;
            case GooglePlayManager.Errors.GET_SESSION_ID:
                PopupManager.instance.ShowPopupMessage(ERROR_GET_SESSION_ID_TITLE, ERROR_GET_SESSION_ID_MESSAGE, PopupMessage.Type.Error);
                break;
            case GooglePlayManager.Errors.GET_ACCOUNT_DATA:
                PopupManager.instance.ShowPopupMessage(ERROR_GET_ACCOUNT_DATA_TITLE, ERROR_GET_ACCOUNT_DATA_MESSAGE, PopupMessage.Type.Error);
                break;
            case GooglePlayManager.Errors.GET_ID_TOKEN:
                PopupManager.instance.ShowPopupMessage(ERROR_GET_ID_TOKEN_TITLE, ERROR_GET_ID_TOKEN_MESSAGE, PopupMessage.Type.Error);
                break;
            default:
                PopupManager.instance.ShowPopupMessage(ERROR_GET_ID_TOKEN_TITLE, ERROR_GET_ID_TOKEN_MESSAGE, PopupMessage.Type.Error);
                break;
        }
    }

    private void OnLoadingCompleted() {
        LoadingManager.instance.Hide();

        SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);

        TutorialManager.instance.StartTutorial();
    }

}
