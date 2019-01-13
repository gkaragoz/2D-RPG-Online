using ShiftServer.Proto.Models;
using UnityEngine;

public class AccountManager : MonoBehaviour {
    
    #region Singleton

    public static AccountManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Task initializationProgress;

    [Header("Debug")]
    [SerializeField]
    private Account _account;

    public Account Account {
        get {
            return _account;
        }
        private set {
            _account = value;
        }
    }

    public void Initialize(Account account) {
        this.Account = account;

        initializationProgress?.Invoke();
    }

}
