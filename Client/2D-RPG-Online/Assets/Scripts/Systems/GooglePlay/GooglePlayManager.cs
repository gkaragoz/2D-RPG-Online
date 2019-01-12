using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System;
using SimpleHTTP;
using ShiftServer.Proto.Models;

public class GooglePlayManager : MonoBehaviour {

    #region Singleton

    public static GooglePlayManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public enum Errors {
        SIGN_IN,
        SIGN_OUT,
        GET_SESSION_ID,
        GET_ACCOUNT_DATA,
        GET_ID_TOKEN
    }

    public delegate void AnyErrorOccuredDelegate(Errors error);
    public AnyErrorOccuredDelegate onAnyErrorOccured;

    public Task initializationProgress;
    public Task signInResponseProgress;
    public Task sessionIdResponseProgress;
    public Task accountDataResponseProgress;

    [Header("Initialization")]
    public string IDTokenURL;
    public string AccountDataURL;

    public class IDTokenData {
        public string id_token;
    }

    public class AccountData {
        public string session_id;
    }

    private const string ATTEMP_TO_SIGN_IN = "ATTEMP to Google Play sign in!";
    private const string ATTEMP_TO_SIGN_OUT = "ATTEMP to Google Play sign out!";
    private const string ATTEMP_TO_GET_SESSION_ID = "ATTEMP to get sessionID!";
    private const string ATTEMP_TO_GET_ACCOUNT_INFO = "ATTEMP to get account informations!";
    private const string ATTEMP_TO_GET_ID_TOKEN = "ATTEMP to get Google Play ID Token!";

    private const string ERROR_SIGN_IN = "ERROR on Google Play sign in!";
    private const string ERROR_SIGN_OUT = "ERROR on Google Play sign out!";
    private const string ERROR_GET_SESSION_ID = "ERROR on getting sessionID!";
    private const string ERROR_GET_ACCOUNT_INFO = "ERROR on getting account informations!";
    private const string ERROR_GET_ID_TOKEN = "ERROR on getting Google Play ID Token!";

    private const string SUCCESS_SIGN_IN = "SUCCESS on Google Play sign in!";
    private const string SUCCESS_SIGN_OUT = "SUCCESS on Google Play sign out!";
    private const string SUCCESS_GET_SESSION_ID = "SUCCESS on getting sessionID!";
    private const string SUCCESS_GET_ACCOUNT_INFO = "SUCCESS on getting account informations!";
    private const string SUCCESS_GET_ID_TOKEN = "SUCCESS on getting Google Play ID Token!";

    public void Initialize() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        initializationProgress?.Invoke();
    }

    public void SignIn() {
        Debug.Log(ATTEMP_TO_SIGN_IN);

        // authenticate user:
        Social.localUser.Authenticate(OnSignInResponse);
    }

    public void UnlockAchievement() {
        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress("Cfjewijawiu_QA", 100.0f, (bool success) => {
            // handle success or failure
        });
    }

    public void IncrementAchievement() {
        // increment achievement (achievement ID "Cfjewijawiu_QA") by 5 steps
        PlayGamesPlatform.Instance.IncrementAchievement(
            "Cfjewijawiu_QA", 5, (bool success) => {
                // handle success or failure
            });
    }

    public void PostAScoreToLeaderboard() {
        // post score 12345 to leaderboard ID "Cfji293fjsie_QA")
        Social.ReportScore(12345, "Cfji293fjsie_QA", (bool success) => {
            // handle success or failure
        });
    }

    public void ShowAchievements() {
        // show achievements UI
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboard() {
        // show leaderboard UI
        // use with non-parameter to get all leaderboards.
        PlayGamesPlatform.Instance.ShowLeaderboardUI("Cfji293fjsie_QA");
    }

    public void GetLeaderboardData() {
        ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        lb.id = "MY_LEADERBOARD_ID";
        lb.LoadScores(ok => {
            if (ok) {
                //LoadUsersAndDisplay(lb);
            } else {
                Debug.Log("Error retrieving leaderboardi");
            }
        });
    }

    public void GetLoadScores() {
        PlayGamesPlatform.Instance.LoadScores(
            "Cfji293fjsie_QA",
            LeaderboardStart.PlayerCentered,
            100,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) => {
                //mStatus = "Leaderboard data valid: " + data.Valid;
                //mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
            });
    }

    public void LoadUsersAndDisplay(ILeaderboard lb) {
        // get the user ids
        List<string> userIds = new List<string>();

        foreach (IScore score in lb.scores) {
            userIds.Add(score.userID);
        }
        // load the profiles and display (or in this case, log)
        Social.LoadUsers(userIds.ToArray(), (users) => {
            string status = "Leaderboard loading: " + lb.title + " count = " +
                lb.scores.Length;
            foreach (IScore score in lb.scores) {
                //IUserProfile user = FindUser(users, score.userID);
                //status += "\n" + score.formattedValue + " by " +
                //    (string)(
                //        (user != null) ? user.userName : "**unk_" + score.userID + "**");
            }
            Debug.Log(status);
        });
    }

    public void GetEmail() {
        // call this from Update()
        Debug.Log("Local user's email is " +
            ((PlayGamesLocalUser)Social.localUser).Email);
    }

    public void LoadFriends() {
        Social.localUser.LoadFriends((ok) => {
            Debug.Log("Friends loaded OK: " + ok);
            foreach (IUserProfile p in Social.localUser.friends) {
                Debug.Log(p.userName + " is a friend");
            }
        });
    }

    public void SignOut() {
        Debug.Log(ATTEMP_TO_SIGN_OUT + Social.localUser.userName);
        // sign out
        PlayGamesPlatform.Instance.SignOut();

        if (Social.localUser.authenticated) {
            Debug.Log(ERROR_SIGN_OUT + Social.localUser.userName);

            onAnyErrorOccured?.Invoke(Errors.SIGN_OUT);
        } else {
            Debug.Log(SUCCESS_SIGN_OUT);
        }
    }

    private IEnumerator IIdTokenPostMethod(IDTokenData data, Action<AuthResponse> callback) {
        AuthResponse authResponse = new AuthResponse();

        Request request = new Request(IDTokenURL)
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

            onAnyErrorOccured?.Invoke(Errors.GET_ID_TOKEN);
            callback(authResponse);
        }
    }

    private IEnumerator IAccountDataPostMethod(AccountData data, Action<Account> callback) {
        Account account = new Account();

        Request request = new Request(AccountDataURL)
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

            onAnyErrorOccured?.Invoke(Errors.GET_ACCOUNT_DATA);
            callback(account);
        }
    }

    private void OnSignInResponse(bool success) {
        signInResponseProgress?.Invoke();

        // handle success or failure
        if (success) {
            Debug.Log(SUCCESS_SIGN_IN + Social.localUser.userName);

            // post IdToken and receive sessionID
            IDTokenData idTokenData = new IDTokenData();
            idTokenData.id_token = PlayGamesPlatform.Instance.GetIdToken();

            if (idTokenData.id_token != null) {
                Debug.Log(ATTEMP_TO_GET_SESSION_ID);
                StartCoroutine(IIdTokenPostMethod(idTokenData, OnSessionIDResponse));
            } else {
                Debug.Log(ERROR_GET_ID_TOKEN);
                onAnyErrorOccured?.Invoke(Errors.GET_ID_TOKEN);
            }
        } else {
            Debug.Log(ERROR_SIGN_IN);
            onAnyErrorOccured?.Invoke(Errors.SIGN_IN);
        }
    }

    private void OnSessionIDResponse(AuthResponse authResponse) {
        sessionIdResponseProgress?.Invoke();

        if (authResponse.success) {
            Debug.Log(SUCCESS_GET_SESSION_ID + authResponse.session);
            Debug.Log(ATTEMP_TO_GET_ACCOUNT_INFO);

            //NetworkManager.instance.SessionID = authResponse.session;

            // post sessionID and receive accountData
            AccountData accountData = new AccountData();
            //accountData.session_id = NetworkManager.instance.SessionID;
            accountData.session_id = authResponse.session;

            StartCoroutine(IAccountDataPostMethod(accountData, OnAccountDataResponse));
        } else {
            // couldn't receive sessionID;
            // retry SignIn and get sessionID

            Debug.Log(ERROR_GET_SESSION_ID);
            onAnyErrorOccured?.Invoke(Errors.GET_SESSION_ID);
            SignOut();
        }
    }

    private void OnAccountDataResponse(Account accountResponse) {
        accountDataResponseProgress?.Invoke();

        Debug.Log(SUCCESS_GET_ACCOUNT_INFO);
    }

}
