using ManaShiftServer.Data.RestModels;
using System;
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

    public LoadingTask initializationProgress;

    public Action<CharacterModel> onCharacterCreated;
    public Action<CharacterModel> onCharacterSelected;

    public CharacterModel SelectedCharacter { get { return _selectedCharacter; } }

    [Header("Initialization")]
    [SerializeField]
    private CharacterCreation _characterCreation;
    [SerializeField]
    private CharacterSelection _characterSelection;

    [Header("Debug")]
    [SerializeField]
    private CharacterModel _selectedCharacter;

    private void Start() {
        SceneController.instance.onSceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName) {
        if (sceneName == "CharacterSelection") {
            Initialize();
        }
    }

    public void Initialize() {
        _characterSelection.Initialize();

        initializationProgress.Invoke();
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
        if (AccountManager.instance.AllCharacters.Count == 0) {
            SelectCharacter(newCharacter);
        }

        onCharacterCreated?.Invoke(newCharacter);
    }

    public CharacterModel GetCharacterModel(string name) {
        for (int ii = 0; ii < AccountManager.instance.AllCharacters.Count; ii++) {
            if (AccountManager.instance.AllCharacters[ii].name == name) {
                return AccountManager.instance.AllCharacters[ii];
            }
        }
        return null;
    }

    public string GetCharacterName(int index) {
        return AccountManager.instance.AllCharacters[index].name;
    } 

}
