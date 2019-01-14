using ShiftServer.Proto.RestModels;
using System;
using UnityEngine;

public class LoginManager : MonoBehaviour {

    #region Singleton

    public static LoginManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Action onLoginCompleted;

    public Action<APIConfig.LoginResults> onLoginResult;

    public Task initializationProgress;
    public Task googlePlaySignInResponseProgress;
    public Task sessionIdResponseProgress;
    public Task accountDataResponseProgress;

    public static string ATTEMP_TO_GET_GUEST_SESSION = "ATTEMP to get Guest Session!";
    public static string ERROR_GET_GUEST_SESSION = "ERROR on getting Guest Session!";
    public static string SUCCESS_GET_GUEST_SESSION = "SUCCESS on getting Guest Session!";

    public void Initialize() {
        GooglePlayManager.instance.onGooglePlaySignInResult += OnGooglePlaySignInResult;
        onLoginResult = OnLoginResult;

        initializationProgress.Invoke();
    }

    public void Login() {
        LoginViaGooglePlayServices();
    }

    private void LoginViaGooglePlayServices() {
        GooglePlayManager.instance.Initialize();
        GooglePlayManager.instance.SignIn();
    }

    private void LoginAsAGuest() {
        RequestGuestAuth requestGuestAuth = new RequestGuestAuth();

        if (GameManager.instance.HasPlayedBefore) {
            requestGuestAuth.guest_id = NetworkManager.UserID;
        } else {
            requestGuestAuth.guest_id = "";
        }

        StartCoroutine(APIConfig.IGuestLoginPostMethod(requestGuestAuth, OnSessionIDResponse));
    }

    private void RequestSessionID() {
        RequestAuth requestAuth = new RequestAuth();
        requestAuth.id_token = GooglePlayManager.ID_TOKEN;

        StartCoroutine(APIConfig.ISessionIDPostMethod(requestAuth, OnSessionIDResponse));
    }

    private void RequestAccountData() {
        RequestAccountData requestAccountData = new RequestAccountData();
        requestAccountData.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.IAccountDataPostMethod(requestAccountData, OnAccountDataResponse));
    }

    private void OnLoginResult(APIConfig.LoginResults result) {
        switch (result) {
            case APIConfig.LoginResults.ERROR_GET_SESSION_ID:
                PopupManager.instance.ShowPopupMessage("ERROR", ((int)result).ToString(), PopupMessage.Type.Error);
                break;
            case APIConfig.LoginResults.ERROR_GET_GUEST_SESSION:
                PopupManager.instance.ShowPopupMessage("ERROR", ((int)result).ToString(), PopupMessage.Type.Error);
                break;
            case APIConfig.LoginResults.ERROR_GET_ACCOUNT_DATA:
                PopupManager.instance.ShowPopupMessage("ERROR", ((int)result).ToString(), PopupMessage.Type.Error);
                break;
            case APIConfig.LoginResults.SUCCESS_GET_SESSION_ID:
                sessionIdResponseProgress.Invoke();
                break;
            case APIConfig.LoginResults.SUCCESS_GET_GUEST_SESSION:
                sessionIdResponseProgress.Invoke();
                break;
            case APIConfig.LoginResults.SUCCESS_GET_ACCOUNT_DATA:
                accountDataResponseProgress.Invoke();
                onLoginCompleted?.Invoke();
                break;
            default:
                PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
                break;
        }
    }

    private void OnGooglePlaySignInResult(GooglePlayManager.Results result) {
        googlePlaySignInResponseProgress.Invoke();

        switch (result) {
            case GooglePlayManager.Results.SUCCESS_SIGN_IN:
                RequestSessionID();
                break;
            case GooglePlayManager.Results.SUCCESS_SIGN_OUT:
                LoginAsAGuest();
                break;
            case GooglePlayManager.Results.ERROR_SIGN_IN:
                LoginAsAGuest();
                break;
            case GooglePlayManager.Results.ERROR_SIGN_OUT:
                PopupManager.instance.ShowPopupMessage("ERROR", ((int)result).ToString(), PopupMessage.Type.Error);
                break;
            default:
                PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
                break;
        }
    }

    private void OnSessionIDResponse(Auth authResponse) {
        if (authResponse.success) {
            Debug.Log(APIConfig.SUCCESS_GET_SESSION_ID + authResponse.session_id);

            NetworkManager.SessionID = authResponse.session_id;
            NetworkManager.UserID = authResponse.user_id;

            RequestAccountData();

            onLoginResult?.Invoke(APIConfig.LoginResults.SUCCESS_GET_SESSION_ID);
        } else {
            Debug.Log(APIConfig.ERROR_GET_SESSION_ID);
            onLoginResult?.Invoke(APIConfig.LoginResults.ERROR_GET_SESSION_ID);
        }
    }

    private void OnAccountDataResponse(Account accountDataResponse) {
        if (accountDataResponse.success) {
            Debug.Log(APIConfig.SUCCESS_GET_ACCOUNT_INFO);

            AccountManager.instance.Initialize(accountDataResponse);

            onLoginResult.Invoke(APIConfig.LoginResults.SUCCESS_GET_ACCOUNT_DATA);
        } else {
            Debug.Log(APIConfig.ERROR_GET_ACCOUNT_INFO);
            onLoginResult.Invoke(APIConfig.LoginResults.ERROR_GET_ACCOUNT_DATA);
        }
    }

}
