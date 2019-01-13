using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System;

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

    public enum Results {
        SUCCESS_SIGN_IN = 00010,
        SUCCESS_SIGN_OUT = 00099,

        ERROR_SIGN_IN = 10010,
        ERROR_SIGN_OUT = 10099
    }

    public Action<Results> onGooglePlaySignInResult;

    public static string ID_TOKEN = "";

    public void Initialize() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignIn() {
        Debug.Log(APIConfig.ATTEMP_TO_GOOGLE_PLAY_SIGN_IN);

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
        Debug.Log(APIConfig.ATTEMP_TO_GOOGLE_PLAY_SIGN_OUT + Social.localUser.userName);
        // sign out
        PlayGamesPlatform.Instance.SignOut();

        if (Social.localUser.authenticated) {
            Debug.Log(APIConfig.ERROR_GOOGLE_PLAY_SIGN_OUT + Social.localUser.userName);

            onGooglePlaySignInResult?.Invoke(Results.SUCCESS_SIGN_OUT);
        } else {
            Debug.Log(APIConfig.SUCCESS_GOOGLE_PLAY_SIGN_OUT);

            onGooglePlaySignInResult?.Invoke(Results.ERROR_SIGN_OUT);
        }
    }

    private void OnSignInResponse(bool success) {
        // handle success or failure
        if (success) {
            Debug.Log(APIConfig.SUCCESS_GOOGLE_PLAY_SIGN_IN + Social.localUser.userName);

            ID_TOKEN = PlayGamesPlatform.Instance.GetIdToken();

            onGooglePlaySignInResult?.Invoke(Results.SUCCESS_SIGN_IN);

        } else {
            Debug.Log(APIConfig.ERROR_GOOGLE_PLAY_SIGN_IN);

            onGooglePlaySignInResult?.Invoke(Results.ERROR_SIGN_IN);
        }
    }

}
