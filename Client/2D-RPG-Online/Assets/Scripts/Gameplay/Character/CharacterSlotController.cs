using ManaShiftServer.Data.RestModels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlotController : MonoBehaviour, IPointerClickHandler {

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI txtName;
    [SerializeField]
    private TextMeshProUGUI txtLevel;

    private CharacterModel _characterModel;

    public void Initialize(CharacterModel characterModel) {
        this._characterModel = characterModel;
        txtName.text = characterModel.name;
        txtLevel.text = "Lv. " + characterModel.level;
    }

    public void OnPointerClick(PointerEventData eventData) {
        TargetIndicator.instance.SetPosition(eventData.pointerCurrentRaycast.gameObject.transform);
    }
}
