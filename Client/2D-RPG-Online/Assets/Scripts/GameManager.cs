﻿using ShiftServer.Proto.Models;
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

    public const string HAS_PLAYED_BEFORE = "HAS_PLAYED_BEFORE";

    public bool HasPlayedBefore {
        get {
            return PlayerPrefs.GetInt(HAS_PLAYED_BEFORE) == 1 ? true : false;
        }
    }

    private const string ATTEMP_TO_CREATE_CHARACTER = "ATTEMP to create character!";
    private const string ERROR_CREATE_CHARACTER = "ERROR on create character!";
    private const string SUCCESS_CREATE_CHARACTER = "SUCCESS on create character!";

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

        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;

        List<Task> tasks = new List<Task>();

        if (HasPlayedBefore) {
            tasks.Add(LoginManager.instance.initializationProgress);
            tasks.Add(LoginManager.instance.googlePlaySignInResponseProgress);
            tasks.Add(LoginManager.instance.sessionIdResponseProgress);
            tasks.Add(LoginManager.instance.accountDataResponseProgress);

            LoadingManager.instance.SetCheckList(tasks);

            LoginManager.instance.Initialize();
            LoginManager.instance.Login();
        } else {
            tasks.Add(LoginManager.instance.initializationProgress);

            LoadingManager.instance.SetCheckList(tasks);

            LoginManager.instance.Initialize();
        }

    }

    private void OnLoadingCompleted() {
        LoadingManager.instance.Hide();

        if (!HasPlayedBefore) {
            PlayerPrefs.SetInt(HAS_PLAYED_BEFORE, 1);

            SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);

            LoginManager.instance.Login();

            TutorialManager.instance.StartTutorial();
        } else {
            //Open Main Menu.
        }
    }

    private void OnCreateCharacterResponse(AddCharResponse createCharacterResponse) {
        if (createCharacterResponse.success) {
            Debug.Log(SUCCESS_CREATE_CHARACTER);
            Debug.Log(createCharacterResponse.character);
        } else {
            Debug.Log(ERROR_CREATE_CHARACTER);
            Debug.Log(createCharacterResponse.error_message);
        }
    }
}
