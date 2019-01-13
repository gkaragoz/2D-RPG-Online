using System;
using ShiftServer.Proto.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : Menu {

    public Action onCharacterCreated;

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI _txtClassDescription;
    [SerializeField]
    private TMP_InputField _inputFieldCharacterName;
    [SerializeField]
    private Button _btnCreateCharacter;
    [SerializeField]
    private Image _imageClassHighlight;

    [Header("Settings")]
    [SerializeField]
    private int _minimumCharacterNameLength;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _selectedClassID;

    public int SelectedClassID {
        get { return _selectedClassID; }
    }

    public string CharacterName {
        get { return _inputFieldCharacterName.text; }
    }

    private void Start() {
        SelectClass(0);
        CheckNameAvailability();
    }

    public void SelectClass(int classID) {
        _selectedClassID = classID;

        if (classID == 0) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Warrior).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Warrior).ClassDescription;
        } else if (classID == 1) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Archer).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Archer).ClassDescription;
        } else if (classID == 2) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Mage).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Mage).ClassDescription;
        } else if (classID == 3) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Priest).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetCharacterClassVisualize(CharacterClassVisualization.Classes.Priest).ClassDescription;
        }
    }

    public void OnInputFieldValueChanged() {
        CheckNameAvailability();
    }

    public void CreateCharacter() {
        APIConfig.CreateCharacterRequest createCharacterRequest = new APIConfig.CreateCharacterRequest();
        createCharacterRequest.char_class = SelectedClassID;
        createCharacterRequest.char_name = CharacterName;
        createCharacterRequest.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ICreateCharacterPostMethod(createCharacterRequest, OnCreateCharacterResponse));
    }

    private void OnCreateCharacterResponse(AddCharResponse createCharacterResponse) {
        if (createCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_CREATE_CHARACTER);

            onCharacterCreated?.Invoke();
        } else {
            Debug.Log(APIConfig.ERROR_CREATE_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", createCharacterResponse.error_message, PopupMessage.Type.Error);
        }
    }

    private void CheckNameAvailability() {
        if (CharacterName.Length < _minimumCharacterNameLength) {
            _btnCreateCharacter.interactable = false;
        } else {
            _btnCreateCharacter.interactable = true;
        }
    }

}
