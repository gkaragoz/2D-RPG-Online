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
    private Account _account;

    public Account Account {
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

    public void Initialize(Account account) {
        this.Account = account;

        CharacterManager.instance.Initialize(account.characters);

        initializationProgress?.Invoke();
        onAccountUpdated?.Invoke();
    }

    private void AddCharacter(Character newCharacter) {
        _account.characters.Add(newCharacter);

        onAccountUpdated?.Invoke();
    }

    private void SelectCharacter(Character selectedCharacter) {
        _account.selectedCharacter = selectedCharacter;
    }

    private void OnCharacterCreated(Character newCharacter) {
        AddCharacter(newCharacter);
    }

    private void OnCharacterSelected(Character selectedCharacter) {
        SelectCharacter(selectedCharacter);
    }

}
