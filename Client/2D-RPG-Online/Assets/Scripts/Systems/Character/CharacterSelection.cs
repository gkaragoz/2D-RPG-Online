using ShiftServer.Proto.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : Menu {

    [Header("Initialization")]
    [SerializeField]
    private Button _btnSelectCharacter;
    [SerializeField]
    private List<CharacterSelectionSlot> _characterSelectionSlots = new List<CharacterSelectionSlot>();

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private string _selectedCharacterName;

    public string SelectedCharacterName {
        get { return _selectedCharacterName; }
    }

    private void Start() {
        _btnSelectCharacter.interactable = false;
    }

    public void Initialize() {
        for (int ii = 0; ii < _characterSelectionSlots.Count; ii++) {
            _characterSelectionSlots[ii].Initialize(CharacterManager.instance.AllCharacters[ii]);
        }
    }

    public void SelectCharacter(string characterName) {
        _selectedCharacterName = characterName;
        _btnSelectCharacter.interactable = true;
    }

    public void SelectCharacter() {
        CharSelectRequest selectCharacterRequest = new CharSelectRequest();
        selectCharacterRequest.char_name = SelectedCharacterName;
        selectCharacterRequest.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(selectCharacterRequest, OnSelectCharacterResponse));
    }

    private void OnSelectCharacterResponse(CharSelectResponse selectCharacterResponse) {
        if (selectCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_SELECT_CHARACTER);

            CharacterManager.instance.SelectCharacter(CharacterManager.instance.GetCharacterModel(_selectedCharacterName));
        } else {
            Debug.Log(APIConfig.ERROR_SELECT_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
        }
    }

}
