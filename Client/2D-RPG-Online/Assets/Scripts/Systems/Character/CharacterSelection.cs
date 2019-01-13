using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class CharacterSelection : Menu {

    [Header("Initialization")]
    [SerializeField]
    private Button _btnSelectCharacter;
    [SerializeField]
    private List<CharacterSelectionSlot> _characterSelectionSlots = new List<CharacterSelectionSlot>();

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _selectedCharacterSlotID;

    public int SelectedCharacterSlotID {
        get { return _selectedCharacterSlotID; }
    }

    private void Start() {
        _btnSelectCharacter.interactable = false;
    }

    public void Initialize() {
        for (int ii = 0; ii < _characterSelectionSlots.Count; ii++) {
            _characterSelectionSlots[ii].Initialize(CharacterManager.instance.AllCharacters[ii]);
        }
    }

    public void SelectClass(int classID) {
        _selectedCharacterSlotID = classID;
        _btnSelectCharacter.interactable = true;
    }

    public void SelectCharacter() {
        APIConfig.SelectCharacterRequest selectCharacterRequest = new APIConfig.SelectCharacterRequest();
        selectCharacterRequest.slot_id = SelectedCharacterSlotID;
        selectCharacterRequest.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(selectCharacterRequest, OnSelectCharacterResponse));
    }

    private void OnSelectCharacterResponse(SelectCharResponse selectCharacterResponse) {
        if (selectCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_SELECT_CHARACTER);

            CharacterManager.instance.SelectCharacter(selectCharacterResponse.character);
        } else {
            Debug.Log(APIConfig.ERROR_SELECT_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", selectCharacterResponse.error_message, PopupMessage.Type.Error);
        }
    }

}
