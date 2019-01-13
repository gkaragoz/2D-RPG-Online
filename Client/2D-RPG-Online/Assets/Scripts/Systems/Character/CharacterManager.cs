using UnityEngine;

public class CharacterManager : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private CharacterCreation _characterCreation;

    public void ShowCharacterCreationMenu() {
        _characterCreation.Show();
    }

    public void HideCharacterCreationMenu() {
        _characterCreation.Hide();
    }

}
