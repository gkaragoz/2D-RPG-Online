using ManaShiftServer.Data.RestModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlotController : MonoBehaviour, IPointerClickHandler {

    public Action<CharacterSlotController> onSelected;

    public int SlotIndex { get { return _slotIndex; } }

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtLevel;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _slotIndex;

    private CharacterModel _characterModel;

    public void Initialize(CharacterModel characterModel, int slotIndex) {
        this._characterModel = characterModel;
        this._slotIndex = slotIndex;
        _txtName.text = characterModel.name;
        _txtLevel.text = "Lv. " + characterModel.level;
    }

    public void OnSelected() {
        TargetIndicator.instance.SetPosition(transform, TargetIndicator.Type.CharacterSelection);
        onSelected?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnSelected();
    }
}
