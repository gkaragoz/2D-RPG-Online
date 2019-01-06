using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignupManager : Menu {

    #region Singleton

    public static SignupManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [SerializeField]
    private TMP_InputField _inputFieldUsername;
    [SerializeField]
    private TMP_InputField _inputFieldPassword;
    [SerializeField]
    private TMP_InputField _inputFieldEmail;
    [SerializeField]
    private Button _btnLogin;
    [SerializeField]
    private Button _btnSignup;

}
