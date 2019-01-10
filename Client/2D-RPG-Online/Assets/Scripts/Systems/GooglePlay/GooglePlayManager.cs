using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class GooglePlayManager : MonoBehaviour {

    private void Start() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            .EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running.
            .WithInvitationDelegate(OnInvitationReceided)
            // registers a callback for turn based match notifications received while the
            // game is not running.
            .WithMatchDelegate(OnMatchNotificationReceived)
            // requests the email address of the player be available.
            // Will bring up a prompt for consent.
            .RequestEmail()
            // requests a server auth code be generated so it can be passed to an
            //  associated back end server application and exchanged for an OAuth token.
            .RequestServerAuthCode(false)
            // requests an ID token be generated.  This OAuth token can be used to
            //  identify the player to other services such as Firebase.
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void Login() {
        // authenticate user:
        Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            Debug.Log("Google Play Login: " + success);
        });
    }

    private void Update() {
        if (Social.localUser.authenticated) {
            Debug.Log("aq");
        }
    }

    private void OnInvitationReceided(Invitation invitation, bool shouldAutoAccept) {
        throw new NotImplementedException();
    }

    private void OnMatchNotificationReceived(TurnBasedMatch match, bool shouldAutoLaunch) {
        throw new NotImplementedException();
    }

}
