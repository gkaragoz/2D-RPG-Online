using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Button btnFacebookLogin;
    [SerializeField]
    private Button btnFacebookLogout;

	private void Start () {
        FB.Init(this.OnInitComplete, this.OnHideUnity);
    }

    private void Update() {
        if (FB.IsInitialized) {
            btnFacebookLogin.interactable = true;
        } else {
            btnFacebookLogin.interactable = false;
        }

        if (FB.IsInitialized && FB.IsLoggedIn) {
            btnFacebookLogout.interactable = true;
        } else {
            btnFacebookLogout.interactable = false;
        }
    }

    public void CallFBLogin() {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
    }

    public void CallFBLogout() {
        FB.LogOut();
    }

    //FB.IsInitialized;
    //FB.IsLoggedIn;

    private void HandleResult(IResult result) {
        if (result == null) {
            Debug.Log("Null Response\n");
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error)) {
            Debug.Log("Error - Check log for details");
            Debug.Log("Error Response:\n" + result.Error);
        } else if (result.Cancelled) {
            Debug.Log("Cancelled - Check log for details");
            Debug.Log("Cancelled Response:\n" + result.RawResult);
        } else if (!string.IsNullOrEmpty(result.RawResult)) {
            Debug.Log("Success - Check log for details");
            Debug.Log("Success Response:\n" + result.RawResult);
        } else {
            Debug.Log("Empty Response\n");
        }

        Debug.Log(result.ToString());
    }

    private void OnInitComplete() {
        Debug.Log("Success - Check log for details");
        Debug.Log("Success Response: OnInitComplete Called\n");
        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);

        Debug.Log(logMessage);

        if (AccessToken.CurrentAccessToken != null) {
            Debug.Log(AccessToken.CurrentAccessToken.ToString());
        }
    }

    private void OnHideUnity(bool isGameShown) {
        Debug.Log("Success - Check log for details");
        Debug.Log(string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown));
        Debug.Log("Is game shown: " + isGameShown);
    }

}
