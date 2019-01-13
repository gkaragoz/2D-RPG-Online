﻿using ShiftServer.Proto.Models;
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

    public static string URL_SessionID = "http://192.168.1.2:5555/api/auth/login";
    public static string URL_AccountData = "http://192.168.1.2:5555/api/user/account";
    public static string URL_GuestLogin = "http://192.168.1.2:5555/api/auth/guestlogin";
    public static string URL_CreateCharacter = "http://192.168.1.2:5555/api/char/add";
    public static string URL_SelectCharacter = "http://192.168.1.2:5555/api/char/select";

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

    public static IEnumerator ISessionIDPostMethod(AuthRequest data, Action<AuthResponse> callback) {
        Debug.Log(ATTEMP_TO_GET_SESSION_ID);

        AuthResponse authResponse = new AuthResponse();

        Request request = new Request(URL_SessionID)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            authResponse = JsonUtility.FromJson<AuthResponse>(resp.Body());
            callback(authResponse);
        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator IAccountDataPostMethod(AccountDataRequest data, Action<AccountModel> callback) {
        Debug.Log(ATTEMP_TO_GET_ACCOUNT_INFO);

        AccountModel accountModel = new AccountModel();

        Request request = new Request(URL_AccountData)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();

        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            accountModel = JsonUtility.FromJson<AccountModel>(resp.Body());
            callback(accountModel);
        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator IGuestLoginPostMethod(GuestAuthRequest data, Action<AuthResponse> callback) {
        Debug.Log(ATTEMP_TO_GET_GUEST_SESSION);

        AuthResponse authResponse = new AuthResponse();

        Request request = new Request(URL_GuestLogin)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            authResponse = JsonUtility.FromJson<AuthResponse>(resp.Body());
            callback(authResponse);
        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator ICreateCharacterPostMethod(CharAddRequest data, Action<CharAddResponse> callback) {
        Debug.Log(ATTEMP_TO_CREATE_CHARACTER);

        CharAddResponse createCharacterResponse = new CharAddResponse();

        Request request = new Request(URL_CreateCharacter)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            createCharacterResponse = JsonUtility.FromJson<CharAddResponse>(resp.Body());
            callback(createCharacterResponse);
        } else {
            Debug.Log("error: " + http.Error());
        }
    }

    public static IEnumerator ISelectCharacterPostMethod(CharSelectRequest data, Action<CharSelectResponse> callback) {
        Debug.Log(ATTEMP_TO_SELECT_CHARACTER);

        CharSelectResponse selectCharacterResponse = new CharSelectResponse();

        Request request = new Request(URL_SelectCharacter)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            selectCharacterResponse = JsonUtility.FromJson<CharSelectResponse>(resp.Body());
            callback(selectCharacterResponse);
        } else {
            Debug.Log("error: " + http.Error());
        }
    }

}
