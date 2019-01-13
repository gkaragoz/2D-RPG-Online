using ShiftServer.Proto.Models;
using SimpleHTTP;
using System;
using System.Collections;
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

    public Action<APIConfig.LoginResults> onLoginResult;

    public Task initializationProgress;
    public Task googlePlaySignInResponseProgress;
    public Task sessionIdResponseProgress;
    public Task accountDataResponseProgress;

    public static string ATTEMP_TO_GET_GUEST_SESSION = "ATTEMP to get Guest Session!";
    public static string ERROR_GET_GUEST_SESSION = "ERROR on getting Guest Session!";
    public static string SUCCESS_GET_GUEST_SESSION = "SUCCESS on getting Guest Session!";

    public void Initialize() {
        GooglePlayManager.instance.onGooglePlaySignInResult = OnGooglePlaySignInResult;
        onLoginResult = OnLoginResult;

        initializationProgress?.Invoke();
    }

    public void Login() {
        LoginViaGooglePlayServices();
    }

    private void LoginViaGooglePlayServices() {
        GooglePlayManager.instance.Initialize();
        GooglePlayManager.instance.SignIn();
    }

    private void LoginAsAGuest() {
        APIConfig.GuestSessionRequest guestSessionRequest = new APIConfig.GuestSessionRequest();
        guestSessionRequest.guest_id = "";

        StartCoroutine(APIConfig.IGuestLoginPostMethod(guestSessionRequest, OnSessionIDResponse));
    }

    private void RequestSessionID() {
        APIConfig.SessionIDRequest sessionIDRequest = new APIConfig.SessionIDRequest();
        sessionIDRequest.id_token = GooglePlayManager.ID_TOKEN;

        StartCoroutine(APIConfig.ISessionIDPostMethod(sessionIDRequest, OnSessionIDResponse));
    }

    private void RequestAccountData() {
        APIConfig.AccountDataRequest accountDataRequest = new APIConfig.AccountDataRequest();
        accountDataRequest.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.IAccountDataPostMethod(accountDataRequest, OnAccountDataResponse));
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
                sessionIdResponseProgress?.Invoke();
                break;
            case APIConfig.LoginResults.SUCCESS_GET_GUEST_SESSION:
                sessionIdResponseProgress?.Invoke();
                break;
            case APIConfig.LoginResults.SUCCESS_GET_ACCOUNT_DATA:
                accountDataResponseProgress?.Invoke();
                break;
            default:
                break;
        }
    }

    private void OnGooglePlaySignInError(GooglePlayManager.Results result) {
        switch (result) {
            case GooglePlayManager.Results.ERROR_SIGN_IN:
                break;
            case GooglePlayManager.Results.ERROR_SIGN_OUT:
                break;
            default:
                break;
        }
    }

    private void OnGooglePlaySignInResult(GooglePlayManager.Results result) {
        googlePlaySignInResponseProgress?.Invoke();

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

    private void OnSessionIDResponse(AuthResponse authResponse) {
        if (authResponse.success) {
            Debug.Log(APIConfig.SUCCESS_GET_SESSION_ID + authResponse.session_id);

            NetworkManager.SessionID = authResponse.session_id;

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
