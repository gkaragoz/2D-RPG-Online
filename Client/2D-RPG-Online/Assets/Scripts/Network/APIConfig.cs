using Newtonsoft.Json;
using ShiftServer.Proto.RestModels;
using SimpleHTTP;
using System;
using System.Collections;
using UnityEngine;

public static class APIConfig {

    public enum LoginResults {
        ERROR_GET_SESSION_ID = 10010,
        ERROR_GET_GUEST_SESSION = 10011,
        ERROR_GET_ACCOUNT_DATA = 10020,

        SUCCESS_GET_SESSION_ID = 00010,
        SUCCESS_GET_GUEST_SESSION = 00011,
        SUCCESS_GET_ACCOUNT_DATA = 00020
    }

    public static string ERROR_INVALID_JSON = "04106";

    public static string URL_SessionID = "http://192.168.1.2:8000/api/auth/login";
    public static string URL_AccountData = "http://192.168.1.2:8000/api/user/account";
    public static string URL_GuestLogin = "http://192.168.1.2:8000/api/auth/guestlogin";
    public static string URL_CreateCharacter = "http://192.168.1.2:8000/api/char/add";
    public static string URL_SelectCharacter = "http://192.168.1.2:8000/api/char/select";

    public static string ATTEMP_TO_GET_GUEST_SESSION = "ATTEMP to get Guest Session!";
    public static string ATTEMP_TO_GOOGLE_PLAY_SIGN_IN = "ATTEMP to Google Play sign in!";
    public static string ATTEMP_TO_GOOGLE_PLAY_SIGN_OUT = "ATTEMP to Google Play sign out!";
    public static string ATTEMP_TO_GET_SESSION_ID = "ATTEMP to get sessionID!";
    public static string ATTEMP_TO_GET_ACCOUNT_INFO = "ATTEMP to get account informations!";
    public static string ATTEMP_TO_GET_GOOGLE_PLAY_ID_TOKEN = "ATTEMP to get Google Play ID Token!";
    public static string ATTEMP_TO_CREATE_CHARACTER = "ATTEMP to create new character!";
    public static string ATTEMP_TO_SELECT_CHARACTER = "ATTEMP to select a character!";

    public static string ERROR_GET_GUEST_SESSION = "ERROR on getting Guest Session!";
    public static string ERROR_GOOGLE_PLAY_SIGN_IN = "ERROR on Google Play sign in!";
    public static string ERROR_GOOGLE_PLAY_SIGN_OUT = "ERROR on Google Play sign out!";
    public static string ERROR_GET_SESSION_ID = "ERROR on getting sessionID!";
    public static string ERROR_GET_ACCOUNT_INFO = "ERROR on getting account informations!";
    public static string ERROR_GET_GOOGLE_PLAY_ID_TOKEN = "ERROR on getting Google Play ID Token!";
    public static string ERROR_CREATE_CHARACTER = "ERROR on create new character!";
    public static string ERROR_SELECT_CHARACTER = "ERROR on select a character!";

    public static string SUCCESS_GET_GUEST_SESSION = "SUCCESS on getting Guest Session!";
    public static string SUCCESS_GOOGLE_PLAY_SIGN_IN = "SUCCESS on Google Play sign in!";
    public static string SUCCESS_GOOGLE_PLAY_SIGN_OUT = "SUCCESS on Google Play sign out!";
    public static string SUCCESS_GET_SESSION_ID = "SUCCESS on getting sessionID!";
    public static string SUCCESS_GET_ACCOUNT_INFO = "SUCCESS on getting account informations!";
    public static string SUCCESS_GET_GOOGLE_PLAY_ID_TOKEN = "SUCCESS on getting Google Play ID Token!";
    public static string SUCCESS_TO_CREATE_CHARACTER = "SUCCESS to create new character!";
    public static string SUCCESS_TO_SELECT_CHARACTER = "SUCCESS to select a character!";

    public static IEnumerator ISessionIDPostMethod(RequestAuth data, Action<Auth> callback) {
        Debug.Log(ATTEMP_TO_GET_SESSION_ID);

        Auth authResponse = new Auth();

        Request request = new Request(URL_SessionID)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            try {
                authResponse = JsonConvert.DeserializeObject<Auth>(resp.Body());

                callback(authResponse);
            } catch (Exception) {
                PopupManager.instance.ShowPopupMessage("ERROR", ERROR_INVALID_JSON, PopupMessage.Type.Error);
                throw;
            }

        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator IAccountDataPostMethod(RequestAccountData data, Action<Account> callback) {
        Debug.Log(ATTEMP_TO_GET_ACCOUNT_INFO);

        Account accountDataResponse = new Account();

        Request request = new Request(URL_AccountData)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();

        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            try {
                accountDataResponse = JsonConvert.DeserializeObject<Account>(resp.Body());

                callback(accountDataResponse);
            } catch (Exception) {
                PopupManager.instance.ShowPopupMessage("ERROR", ERROR_INVALID_JSON, PopupMessage.Type.Error);
                throw;
            }

        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator IGuestLoginPostMethod(RequestGuestAuth data, Action<Auth> callback) {
        Debug.Log(ATTEMP_TO_GET_GUEST_SESSION);

        Auth authResponse = new Auth();

        Request request = new Request(URL_GuestLogin)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            try {
                authResponse = JsonConvert.DeserializeObject<Auth>(resp.Body());

                callback(authResponse);
            } catch (Exception) {
                PopupManager.instance.ShowPopupMessage("ERROR", ERROR_INVALID_JSON, PopupMessage.Type.Error);
                throw;
            }

        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator ICreateCharacterPostMethod(RequestCharAdd data, Action<CharAdd> callback) {
        Debug.Log(ATTEMP_TO_CREATE_CHARACTER);

        CharAdd createdCharacterResponse = new CharAdd();

        Request request = new Request(URL_CreateCharacter)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            try {
                createdCharacterResponse = JsonConvert.DeserializeObject<CharAdd>(resp.Body());

                callback(createdCharacterResponse);
            } catch (Exception) {
                PopupManager.instance.ShowPopupMessage("ERROR", ERROR_INVALID_JSON, PopupMessage.Type.Error);
                throw;
            }

        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator ISelectCharacterPostMethod(RequestCharSelect data, Action<CharSelect> callback) {
        Debug.Log(ATTEMP_TO_SELECT_CHARACTER);

        CharSelect selectedCharacterResponse = new CharSelect();

        Request request = new Request(URL_SelectCharacter)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            try {
                selectedCharacterResponse = JsonConvert.DeserializeObject<CharSelect>(resp.Body());

                callback(selectedCharacterResponse);
            } catch (Exception) {
                PopupManager.instance.ShowPopupMessage("ERROR", ERROR_INVALID_JSON, PopupMessage.Type.Error);
                throw;
            }

        } else {
            Debug.Log("error: " + http.Error());
        }
    }

}
