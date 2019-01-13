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

    public Action<Character> onCharacterCreated;
    public Action<Character> onCharacterSelected;

    [Header("Initialization")]
    [SerializeField]
    private CharacterCreation _characterCreation;
    [SerializeField]
    private CharacterSelection _characterSelection;

    [Header("Debug")]
    [SerializeField]
    private Character _selectedCharacter;
    [SerializeField]
    private List<Character> _allCharacters = new List<Character>();

    public void ShowCharacterCreationMenu() {
        _characterCreation.Show();
    }

    public void HideCharacterCreationMenu() {
        _characterCreation.Hide();
    }

    public void SelectCharacter(Character character) {
        _selectedCharacter = character;

        onCharacterSelected?.Invoke(character);
    }

    public void AddCharacter(Character newCharacter) {
        _allCharacters.Add(newCharacter);

        onCharacterCreated?.Invoke(newCharacter);
    }

}
