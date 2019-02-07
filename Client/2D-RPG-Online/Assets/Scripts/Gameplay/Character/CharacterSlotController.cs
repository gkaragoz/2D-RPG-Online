using ManaShiftServer.Data.RestModels;
using TMPro;
using UnityEngine;

public class CharacterSlotController : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI txtName;
    [SerializeField]
    private TextMeshProUGUI txtLevel;

    private CharacterModel _characterModel;

    public void Initialize(CharacterModel characterModel) {
        this._characterModel = characterModel;
        txtName.text = characterModel.name;
        txtLevel.name = characterModel.level.ToString();
    }

}
