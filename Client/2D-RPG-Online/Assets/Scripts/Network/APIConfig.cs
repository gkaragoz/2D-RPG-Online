using ShiftServer.Proto.Models;
using SimpleHTTP;
using System;
using System.Collections;
using UnityEngine;

public static class APIConfig {

    public static string URL_SessionID = "http://192.168.1.2:5555/api/auth/login";
    public static string URL_AccountData = "http://192.168.1.2:5555/api/user/account";
    public static string URL_GuestLogin = "http://192.168.1.2:5555/api/auth/guestlogin";
    public static string URL_CreateCharacter = "http://192.168.1.2:5555/api/char/add";

    public class SessionIDRequest {
        public string id_token;
    }

    public class AccountDataRequest {
        public string session_id;
    }

    public class GuestSessionRequest {
        public string guest_id;
    }

    public class CreateCharacterRequest {
        public string session_id;
        public string char_name;
        public int char_class;
    }

    public static IEnumerator ISessionIDPostMethod(SessionIDRequest data, Action<AuthResponse> callback) {
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

            GooglePlayManager.instance.onAnyErrorOccured?.Invoke(GooglePlayManager.Errors.GET_ID_TOKEN);
            callback(authResponse);
        }
    }

    public static IEnumerator IAccountDataPostMethod(AccountDataRequest data, Action<Account> callback) {
        Account account = new Account();

        Request request = new Request(URL_AccountData)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();

        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            account = JsonUtility.FromJson<Account>(resp.Body());
            callback(account);
        } else {
            Debug.Log("error: " + http.Error());

            GooglePlayManager.instance.onAnyErrorOccured?.Invoke(GooglePlayManager.Errors.GET_ACCOUNT_DATA);
            callback(account);
        }
    }

    public static IEnumerator IGuestLoginPostMethod(GuestSessionRequest data, Action<AuthResponse> callback) {
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

            GooglePlayManager.instance.onAnyErrorOccured?.Invoke(GooglePlayManager.Errors.GET_GUEST_SESSION);
            callback(authResponse);
        }
    }

    public static IEnumerator ICreateCharacterPostMethod(CreateCharacterRequest data, Action<AddCharResponse> callback) {
        AddCharResponse createCharacterResponse = new AddCharResponse();

        Request request = new Request(URL_CreateCharacter)
            .Post(RequestBody.From(data));

        request.Timeout(10);

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            createCharacterResponse = JsonUtility.FromJson<AddCharResponse>(resp.Body());
            callback(createCharacterResponse);
        } else {
            Debug.Log("error: " + http.Error());

            //GooglePlayManager.instance.onAnyErrorOccured?.Invoke(GooglePlayManager.Errors.GET_ID_TOKEN);
            callback(createCharacterResponse);
        }
    }

}
