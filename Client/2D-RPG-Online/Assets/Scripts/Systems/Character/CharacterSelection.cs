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
    private GameObject _selectedCharacterObject;
    [SerializeField]
    [Utils.ReadOnly]
    private string _selectedCharacterName;
    [Utils.ReadOnly]
    [SerializeField]
    private GameObject[] _characterSlots;
    
    public string SelectedCharacterName {
        get { return _selectedCharacterName; }
    }

    public void Initialize() {
        _characterSlots = GameObject.FindGameObjectsWithTag("CharacterSelectionSlot");

        for (int ii = 0; ii < AccountManager.instance.AllCharacters.Count; ii++) {
            _characterSlots[ii].transform.Find("Spot Light").gameObject.SetActive(true);
            Transform slotCharacterParent = Instantiate(_slotCharacterPrefab, _characterSlots[ii].transform).transform;

            PlayerClass playerClass = (PlayerClass)AccountManager.instance.AllCharacters[ii].class_index;

            switch (playerClass) {
                case PlayerClass.Warrior:
                    Instantiate(_warriorPrefab, slotCharacterParent);
                    break;
                case PlayerClass.Archer:
                    Instantiate(_archerPrefab, slotCharacterParent);
                    break;
                case PlayerClass.Mage:
                    Instantiate(_magePrefab, slotCharacterParent);
                    break;
                case PlayerClass.Priest:
                    Instantiate(_priestPrefab, slotCharacterParent);
                    break;
                default:
                    Debug.LogError("CLASS NOT FOUND!");
                    break;
            }
        }
    }

    public void SelectCharacter(int selectedIndex) {
        _selectedCharacterName = CharacterManager.instance.GetCharacterName(selectedIndex);
    }

    public void SubmitSelectCharacter() {
        RequestCharSelect RequestSelectCharacter = new RequestCharSelect();
        RequestSelectCharacter.char_name = SelectedCharacterName;
        RequestSelectCharacter.session_id = NetworkManager.SessionID;

        StartCoroutine(APIConfig.ISelectCharacterPostMethod(RequestSelectCharacter, OnSelectCharacterResponse));
    }

    private void OnSelectCharacterResponse(CharSelect selectedCharacterResponse) {
        if (selectedCharacterResponse.success) {
            Debug.Log(APIConfig.SUCCESS_TO_SELECT_CHARACTER);

            CharacterManager.instance.SelectCharacter(CharacterManager.instance.GetCharacterModel(_selectedCharacterName));
        } else {
            Debug.Log(APIConfig.ERROR_SELECT_CHARACTER);

            PopupManager.instance.ShowPopupMessage("ERROR", "Unknown", PopupMessage.Type.Error);
        }
    }

}
