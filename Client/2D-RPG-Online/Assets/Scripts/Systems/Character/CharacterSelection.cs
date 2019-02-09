using ManaShiftServer.Data.RestModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : Menu {

    [SerializeField]
    private GameObject _warriorPrefab;
    [SerializeField]
    private GameObject _archerPrefab;
    [SerializeField]
    private GameObject _magePrefab;
    [SerializeField]
    private GameObject _priestPrefab;
    [SerializeField]
    private CharacterSlotController[] _characterSlotControllers;
    [SerializeField]
    private Button _btnImReady;
    [SerializeField]
    private Button _btnCreateCharacter;
    [SerializeField]
    private Button _btnBack;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private CharacterSlotController _selectedSlot;
    [Utils.ReadOnly]
    [SerializeField]
    private GameObject _selectionSlotsParent;

    public void Initialize() {
        if (_selectionSlotsParent == null) {
            _selectionSlotsParent = GameObject.Find("SelectionSlots");
            subContainer = _selectionSlotsParent;
        }

        if (!AccountManager.instance.HasCharacter) {
            _btnBack.gameObject.SetActive(false);
        } else {
            _btnBack.gameObject.SetActive(true);
        }

        _characterSlotControllers = _selectionSlotsParent.GetComponentsInChildren<CharacterSlotController>();

        for (int ii = 0; ii < AccountManager.instance.AllCharacters.Count; ii++) {
            if (_characterSlotControllers[ii].HasInitialized) {
                continue;
            }

            PlayerClass playerClass = (PlayerClass)AccountManager.instance.AllCharacters[ii].class_index;

            GameObject _characterObject = null;
            
            switch (playerClass) {
                case PlayerClass.Warrior:
                    _characterObject = Instantiate(_warriorPrefab, _characterSlotControllers[ii].transform);
                    break;
                case PlayerClass.Archer:
                    _characterObject = Instantiate(_archerPrefab, _characterSlotControllers[ii].transform);
                    break;
                case PlayerClass.Mage:
                    _characterObject = Instantiate(_magePrefab, _characterSlotControllers[ii].transform);
                    break;
                case PlayerClass.Priest:
                    _characterObject = Instantiate(_priestPrefab, _characterSlotControllers[ii].transform);
                    break;
                default:
                    Debug.LogError("CLASS NOT FOUND!");
                    break;
            }

            _characterSlotControllers[ii].Initialize(AccountManager.instance.AllCharacters[ii], ii, _characterObject);
        }

        for (int ii = 0; ii < _characterSlotControllers.Length; ii++) {
            _characterSlotControllers[ii].transform.parent.GetComponentInChildren<Canvas>().enabled = !_characterSlotControllers[ii].HasInitialized;
            _characterSlotControllers[ii].onSelected += SelectCharacter;

            if (_characterSlotControllers[ii].HasInitialized) {
                if (ii == 0) {
                    TargetIndicator.instance.SetPosition(_characterSlotControllers[ii].transform, TargetIndicator.Type.CharacterSelection);
                    SlotHighlighter.instance.SetPosition(_characterSlotControllers[ii].transform);
                    SelectCharacter(_characterSlotControllers[ii]);
                } else {
                    _characterSlotControllers[ii].Sit();
                }
            }
        }
    }

    public void SelectCharacter(CharacterSlotController selectedSlot) {
        if (this._selectedSlot != null) {
            if (this._selectedSlot != selectedSlot) {
                this._selectedSlot.Sit();
            }
        }

        this._selectedSlot = selectedSlot;

        SetSubmitButton();
    }

    public void SubmitSelectCharacter() {
        RequestCharSelect RequestSelectCharacter = new RequestCharSelect();
        RequestSelectCharacter.char_name = CharacterManager.instance.GetCharacterName(this._selectedSlot.SlotIndex);
        RequestSelectCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(RequestSelectCharacter, OnSelectCharacterResponse));
    }

    private void SetSubmitButton() {
        if (this._selectedSlot.HasInitialized) {
            _btnCreateCharacter.gameObject.SetActive(false);
            _btnImReady.gameObject.SetActive(true);
        } else {
            _btnCreateCharacter.gameObject.SetActive(true);
            _btnImReady.gameObject.SetActive(false);
        }
    }

    private void OnSelectCharacterResponse(CharSelect selectedCharacterResponse) {
        if (selectedCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_SELECT_CHARACTER);

            string selectedCharacterName = CharacterManager.instance.GetCharacterName(this._selectedSlot.SlotIndex);
            CharacterManager.instance.SelectCharacter(CharacterManager.instance.GetCharacterModel(selectedCharacterName));
        } else {
            Debug.Log(APIConfig.ERROR_SELECT_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
        }
    }

}
