using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using SimpleHTTP;

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

    [Header("Initialization")]
    public string IDTokenURL;
    public string AccountDataURL;

    public class IDTokenData {
        public string id_token;
    }

    public class AccountData {
        public string session_id;
    }

    private void Start() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignIn() {
        // authenticate user:
        Social.localUser.Authenticate((bool signInSuccess) => {
            // handle success or failure
            if (signInSuccess) {
                // post IdToken and receive sessionID
                IDTokenData idTokenData = new IDTokenData();
                idTokenData.id_token = PlayGamesPlatform.Instance.GetIdToken();

                StartCoroutine(IIdTokenPostMethod(idTokenData, (string sessionID) => {
                    if (sessionID != string.Empty) {
                        NetworkManager.instance.SessionID = sessionID;

                        // post sessionID and receive accountData
                        AccountData accountData = new AccountData();
                        accountData.session_id = NetworkManager.instance.SessionID;

                        StartCoroutine(IAccountDataPostMethod(accountData, (bool accountDataSuccess) => {
                            
                        }));
                    } else {
                        // couldn't receive sessionID;
                        // retry SignIn and get sessionID
                    }
                }));
            }
        });
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
        // sign out
        PlayGamesPlatform.Instance.SignOut();
    }

    private IEnumerator IIdTokenPostMethod(IDTokenData data, Action<string> callback) {
        Request request = new Request(IDTokenURL)
            .Post(RequestBody.From(data));

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            //AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(resp.Body());
            //callback(authRespone.success);
        } else {
            Debug.Log("error: " + http.Error());
            //callback(false);
        }
    }

    private IEnumerator IAccountDataPostMethod(AccountData data, Action<bool> callback) {
        Request request = new Request(AccountDataURL)
            .Post(RequestBody.From(data));

        Client http = new Client();
        yield return http.Send(request);

        if (http.IsSuccessful()) {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());

            //AccountData accountData = JsonUtility.FromJson<AccountData>(resp.Body());
        } else {
            Debug.Log("error: " + http.Error());
        }

        callback(http.IsSuccessful());
    }

}
