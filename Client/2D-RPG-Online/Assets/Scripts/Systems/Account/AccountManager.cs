using ManaShiftServer.Data.RestModels;
using System;
using System.Collections.Generic;
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

    public LoadingTask initializationProgress;

    public Action onAccountManagerInitialized;
    public Action onAccountUpdated;

    public List<CharacterModel> AllCharacters { get { return _account.characters; } }
    public string SelectedCharacterName { get { return _account.selected_char_name; } }
    public bool HasCharacter { get { return _account.characters.Count > 0 ? true : false; } }
    public int Gold { get { return _account.gold; } }
    public int Gem { get { return _account.gem; } }

    [Header("Debug")]
    [SerializeField]
    private Account _account;

    private void Start() {
        CharacterManager.instance.onCharacterCreated += OnCharacterCreated;
        CharacterManager.instance.onCharacterSelected += OnCharacterSelected;
    }

    public void Initialize(Account account) {
        this._account = account;

        initializationProgress.Complete();
        onAccountManagerInitialized?.Invoke();
    }

    public CharacterModel GetSelectedCharacter() {
        if (SelectedCharacterName == "" || HasCharacter == false) {
            return null;
        }

        return GetCharacter(SelectedCharacterName);
    }

    public CharacterModel GetCharacter(string characterName) {
        for (int ii = 0; ii < AllCharacters.Count; ii++) {
            if (AllCharacters[ii].name == characterName) {
                return AllCharacters[ii];
            }
        }
        return null;
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

}
