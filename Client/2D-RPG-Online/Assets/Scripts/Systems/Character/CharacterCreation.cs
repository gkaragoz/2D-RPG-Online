using ManaShiftServer.Data.RestModels;
using System.Collections.Generic;
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

    [Header("Settings")]
    [SerializeField]
    private int _minimumCharacterNameLength;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private CharacterSlotController _selectedSlot;
    [Utils.ReadOnly]
    [SerializeField]
    private CharacterSlotController[] _characterSlotControllers;
    [Utils.ReadOnly]
    [SerializeField]
    private GameObject _characterCreationObjectParent;

    public string CharacterName {
        get { return _inputFieldCharacterName.text; }
    }

    public void Initialize(bool show) {
        _characterCreationObjectParent = GameObject.Find("CreationSlots");

        if (!show) {
            _characterCreationObjectParent.SetActive(false);
            return;
        }

        Show();

        _characterSlotControllers = _characterCreationObjectParent.GetComponentsInChildren<CharacterSlotController>();

        for (int ii = 0; ii < _characterSlotControllers.Length; ii++) {
            _characterSlotControllers[ii].onSelected += SelectClass;

            if (ii == 0) {
                TargetIndicator.instance.SetPosition(_characterSlotControllers[ii].transform, TargetIndicator.Type.CharacterSelection);
                SlotHighlighter.instance.SetPosition(_characterSlotControllers[ii].transform);
                SelectClass(_characterSlotControllers[ii]);
            } else {
                _characterSlotControllers[ii].Sit();
            }
        }

        CheckNameAvailability();
    }

    public void SelectClass(CharacterSlotController selectedSlot) {
        if (this._selectedSlot != null) {
            if (this._selectedSlot != selectedSlot) {
                this._selectedSlot.Sit();
            }
        }

        this._selectedSlot = selectedSlot;
    }

    public void OnInputFieldValueChanged() {
        CheckNameAvailability();
    }

    public void CreateCharacter() {
        RequestCharAdd requestCreateCharacter = new RequestCharAdd();
        requestCreateCharacter.class_index = this._selectedSlot.SlotIndex;
        requestCreateCharacter.char_name = CharacterName;
        requestCreateCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ICreateCharacterPostMethod(requestCreateCharacter, OnCreateCharacterResponse));
    }

    private void OnCreateCharacterResponse(CharAdd createdCharacterResponse) {
        if (createdCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_CREATE_CHARACTER);

            CharacterManager.instance.AddCharacter(createdCharacterResponse.character);
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
