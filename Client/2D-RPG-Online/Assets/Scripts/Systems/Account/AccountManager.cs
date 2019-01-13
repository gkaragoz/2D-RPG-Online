using ShiftServer.Proto.Models;
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
    private AccountModel _account;

    public AccountModel Account {
        get {
            return _account;
        }
        private set {
            _account = value;
        }
    }

    private void Start() {
        CharacterManager.instance.onCharacterCreated = OnCharacterCreated;
        CharacterManager.instance.onCharacterSelected = OnCharacterSelected;
    }

    public void Initialize(AccountModel account) {
        this.Account = account;

        CharacterManager.instance.Initialize(account.characters, account.selected_char_name);

        initializationProgress?.Invoke();
        onAccountUpdated?.Invoke();
    }

    private void AddCharacter(CharacterModel newCharacter) {
        _account.characters.Add(newCharacter);

        onAccountUpdated?.Invoke();
    }

    private void SelectCharacter(CharacterModel selectedCharacter) {
        _account.selected_char_name = selectedCharacter.name;
    }

    private void OnCharacterCreated(CharacterModel newCharacter) {
        AddCharacter(newCharacter);
    }

    private void OnCharacterSelected(CharacterModel selectedCharacter) {
        SelectCharacter(selectedCharacter);
    }

}
