using ShiftServer.Proto.RestModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : Menu {

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
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Warrior).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Warrior).ClassDescription;
        } else if (classID == 1) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Archer).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Archer).ClassDescription;
        } else if (classID == 2) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Mage).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Mage).ClassDescription;
        } else if (classID == 3) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Priest).ClassFrameColor;
            _txtClassDescription.text = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Priest).ClassDescription;
        }
    }

    public void OnInputFieldValueChanged() {
        CheckNameAvailability();
    }

    public void CreateCharacter() {
        RequestCharAdd requestCreateCharacter = new RequestCharAdd();
        requestCreateCharacter.class_index = SelectedClassID;
        requestCreateCharacter.char_name = CharacterName;
        requestCreateCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ICreateCharacterPostMethod(requestCreateCharacter, OnCreateCharacterResponse));
    }

    private void OnCreateCharacterResponse(CharAdd createdCharacterResponse) {
        if (createdCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_CREATE_CHARACTER);

            CharacterManager.instance.AddCharacter(createdCharacterResponse.character);
            CharacterManager.instance.SelectCharacter(createdCharacterResponse.character);
        } else {
            Debug.Log(APIConfig.ERROR_CREATE_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", createdCharacterResponse.error_message, PopupMessage.Type.Error);
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
