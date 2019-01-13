using ShiftServer.Proto.RestModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    #region Singleton

    public static CharacterManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Task initializationProgress;

    public Action<CharacterModel> onCharacterCreated;
    public Action<CharacterModel> onCharacterSelected;

    public List<CharacterModel> AllCharacters { get { return _allCharacters; } }
    public CharacterModel SelectedCharacter { get { return _selectedCharacter; } }

    [Header("Initialization")]
    [SerializeField]
    private CharacterCreation _characterCreation;
    [SerializeField]
    private CharacterSelection _characterSelection;

    [Header("Debug")]
    [SerializeField]
    private CharacterModel _selectedCharacter;
    [SerializeField]
    private List<CharacterModel> _allCharacters = new List<CharacterModel>();

    public void Initialize(List<CharacterModel> characters, CharacterModel _selectedCharacter) {
        this._allCharacters = characters;
        this._selectedCharacter = _selectedCharacter;

        _characterSelection.Initialize();

        initializationProgress?.Invoke();
    }

    public void ShowCharacterCreationMenu() {
        _characterCreation.Show();
    }

    public void HideCharacterCreationMenu() {
        _characterCreation.Hide();
    }

    public void ShowCharacterSelectionMenu() {
        _characterSelection.Show();
    }

    public void HideCharacterSelectionMenu() {
        _characterSelection.Hide();
    }

    public void SelectCharacter(CharacterModel selectedCharacter) {
        this._selectedCharacter = selectedCharacter;

        onCharacterSelected?.Invoke(_selectedCharacter);
    }

    public void AddCharacter(CharacterModel newCharacter) {
        AllCharacters.Add(newCharacter);

        onCharacterCreated?.Invoke(newCharacter);
    }

    public CharacterModel GetCharacterModel(string name) {
        for (int ii = 0; ii < _allCharacters.Count; ii++) {
            if (_allCharacters[ii].name == name) {
                return _allCharacters[ii];
            }
        }
        return null;
    }

}
