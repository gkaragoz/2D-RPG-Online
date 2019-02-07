using ManaShiftServer.Data.RestModels;
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
    private int _selectedIndex;
    [Utils.ReadOnly]
    [SerializeField]
    private GameObject[] _characterSlots;

    public void Initialize() {
        _characterSlots = GameObject.FindGameObjectsWithTag("CharacterSelectionSlot");

        for (int ii = 0; ii < AccountManager.instance.AllCharacters.Count; ii++) {
            Transform slotCharacterParent = Instantiate(_slotCharacterPrefab, _characterSlots[ii].transform).transform;

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

            slotCharacterParent.GetComponent<CharacterSlotController>().Initialize(AccountManager.instance.AllCharacters[ii]);
        }

        SelectCharacter(0);
    }

    public void SelectCharacter(int index) {
        _characterSlots[_selectedIndex].transform.Find("Spot Light").gameObject.SetActive(false);
        _selectedIndex = index;
        _characterSlots[_selectedIndex].transform.Find("Spot Light").gameObject.SetActive(true);
    }

    public void SubmitSelectCharacter() {
        RequestCharSelect RequestSelectCharacter = new RequestCharSelect();
        RequestSelectCharacter.char_name = CharacterManager.instance.GetCharacterName(_selectedIndex);
        RequestSelectCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(RequestSelectCharacter, OnSelectCharacterResponse));
    }

    private void OnSelectCharacterResponse(CharSelect selectedCharacterResponse) {
        if (selectedCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_SELECT_CHARACTER);

            string selectedCharacterName = CharacterManager.instance.GetCharacterName(_selectedIndex);
            CharacterManager.instance.SelectCharacter(CharacterManager.instance.GetCharacterModel(selectedCharacterName));
        } else {
            Debug.Log(APIConfig.ERROR_SELECT_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
        }
    }

}
