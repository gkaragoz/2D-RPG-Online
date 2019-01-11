using System;
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
        LoadingManager.instance.SetCheckList(3);

        GooglePlayManager.instance.Initialize(OnGooglePlayInitialized);
        GooglePlayManager.instance.SignIn(OnSignInStatus);

        LoadingManager.instance.Progress("Complete!");
    }

    private void OnSignInStatus(bool success) {
        if (success) {
            LoadingManager.instance.Progress("Sign in completed!");
        } else {
            Debug.Log("FLOW FAILED: Sign in into Google Play Services!");
        }
    }

    private void OnGooglePlayInitialized() {
        LoadingManager.instance.Progress("GPGS initialized");
    }

}
