using ShiftServer.Proto.RestModels;
using System;
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

    public Action onAccountUpdated;

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

    private const string ON_ACCOUNT_JOIN = "Trying to get account infos!";
    private const string ON_ACCOUNT_JOIN_FAILED = "Account join failed!";

    private void Start() {
        CharacterManager.instance.onCharacterCreated += OnCharacterCreated;
        CharacterManager.instance.onCharacterSelected += OnCharacterSelected;

        NetworkManager.mss.AddEventListener(MSServerEvent.AccountJoin, OnAccountJoined);
        NetworkManager.mss.AddEventListener(MSServerEvent.AccountJoinFailed, OnAccountJoinFailed);
    }

    public void Initialize(Account account) {
        this.Account = account;

        initializationProgress.Invoke();
        onAccountUpdated?.Invoke();
    }

    private void AddCharacter(CharacterModel newCharacter) {
        _account.characters.Add(newCharacter);

        onAccountUpdated?.Invoke();
    }

    private void SelectCharacter(CharacterModel selectedCharacter) {
        _account.selected_char_name = selectedCharacter.name;

        onAccountUpdated?.Invoke();
    }

    private void OnCharacterCreated(CharacterModel newCharacter) {
        AddCharacter(newCharacter);
    }

    private void OnCharacterSelected(CharacterModel selectedCharacter) {
        SelectCharacter(selectedCharacter);
    }

    private void OnAccountJoined(ShiftServerData data) {
        Debug.Log(ON_ACCOUNT_JOIN + data);
    }

    private void OnAccountJoinFailed(ShiftServerData data) {
        Debug.Log(ON_ACCOUNT_JOIN_FAILED + data);
    }

}
