using ManaShiftServer.Data.RestModels;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : Menu {

    [SerializeField]
    private GameObject _slotCharacterPrefab;
    [SerializeField]
    private GameObject _warriorPrefab;
    [SerializeField]
    private GameObject _archerPrefab;
    [SerializeField]
    private GameObject _magePrefab;
    [SerializeField]
    private GameObject _priestPrefab;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private CharacterSlotController _selectedSlot;
    [Utils.ReadOnly]
    [SerializeField]
    private List<CharacterSlotController> _characterSlotControllers = new List<CharacterSlotController>();
    [Utils.ReadOnly]
    [SerializeField]
    private GameObject[] _characterSlotPositions;

    public void Initialize() {
        _characterSlotPositions = GameObject.FindGameObjectsWithTag("CharacterSelectionSlot");

        for (int ii = 0; ii < AccountManager.instance.AllCharacters.Count; ii++) {
            Transform slotCharacterParent = Instantiate(_slotCharacterPrefab, _characterSlotPositions[ii].transform).transform;

            PlayerClass playerClass = (PlayerClass)AccountManager.instance.AllCharacters[ii].class_index;

            GameObject _characterObject = null;
            
            switch (playerClass) {
                case PlayerClass.Warrior:
                    _characterObject = Instantiate(_warriorPrefab, slotCharacterParent);
                    break;
                case PlayerClass.Archer:
                    _characterObject = Instantiate(_archerPrefab, slotCharacterParent);
                    break;
                case PlayerClass.Mage:
                    _characterObject = Instantiate(_magePrefab, slotCharacterParent);
                    break;
                case PlayerClass.Priest:
                    _characterObject = Instantiate(_priestPrefab, slotCharacterParent);
                    break;
                default:
                    Debug.LogError("CLASS NOT FOUND!");
                    break;
            }

            _characterSlotControllers.Add(slotCharacterParent.GetComponent<CharacterSlotController>());
            _characterSlotControllers[ii].Initialize(AccountManager.instance.AllCharacters[ii], ii, _characterObject);
            _characterSlotControllers[ii].onSelected += SelectCharacter;
        }

        for (int ii = 0; ii < _characterSlotControllers.Count; ii++) {
            if (ii == 0) {
                continue;
            }
            _characterSlotControllers[ii].Sit();
        }

        SelectCharacter(_characterSlotControllers[0]);
    }

    public void SelectCharacter(CharacterSlotController selectedSlot) {
        if (this._selectedSlot != null) {
            this._selectedSlot.Sit();
        }

        this._selectedSlot = selectedSlot;
        SlotHighlighter.instance.SetPosition(this._selectedSlot.transform);
    }

    public void SubmitSelectCharacter() {
        RequestCharSelect RequestSelectCharacter = new RequestCharSelect();
        RequestSelectCharacter.char_name = CharacterManager.instance.GetCharacterName(this._selectedSlot.SlotIndex);
        RequestSelectCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(RequestSelectCharacter, OnSelectCharacterResponse));
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
