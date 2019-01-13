using ShiftServer.Proto.Models;
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
    public Action<string> onCharacterSelected;

    public List<CharacterModel> AllCharacters { get { return _allCharacters; } }

    [Header("Initialization")]
    [SerializeField]
    private CharacterCreation _characterCreation;
    [SerializeField]
    private CharacterSelection _characterSelection;

    [Header("Debug")]
    [SerializeField]
    private string _selectedCharacterName;
    [SerializeField]
    private List<CharacterModel> _allCharacters = new List<CharacterModel>();

    public void Initialize(List<CharacterModel> characters, string selectedCharacterName) {
        _allCharacters = characters;
        _selectedCharacterName = selectedCharacterName;

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

    public void SelectCharacter(string characterName) {
        _selectedCharacterName = characterName;

        onCharacterSelected?.Invoke(characterName);
    }

    public void AddCharacter(CharacterModel newCharacter) {
        AllCharacters.Add(newCharacter);

        onCharacterCreated?.Invoke(newCharacter);
    }

}
