using ManaShiftServer.Data.RestModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlotController : MonoBehaviour, IPointerClickHandler {

    public Action<CharacterSlotController> onSelected;

    public bool HasInitialized { get; private set; }

    public int SlotIndex { get { return _slotIndex; } }

    [Header("Initialization")]
    [SerializeField]
    private int _slotIndex;
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtLevel;
    [SerializeField]
    private CharacterAnimator _characterAnimator;
    [SerializeField]
    private GameObject _characterFake;

    private CharacterModel _characterModel;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            _characterAnimator.OnMove(Vector3.forward);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            _characterAnimator.OnHit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _characterAnimator.OnAttack();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            _characterAnimator.Sit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            _characterAnimator.StandUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            _characterAnimator.OnDeath();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            _characterAnimator.Reset();
        }
    }

    public void Initialize(CharacterModel characterModel, int slotIndex, GameObject characterObject) {
        this._characterModel = characterModel;
        this._slotIndex = slotIndex;
        _characterFake.gameObject.SetActive(false);
        _txtName.gameObject.SetActive(true);
        _txtName.text = characterModel.name;
        _txtLevel.gameObject.SetActive(true);
        _txtLevel.text = "Lv. " + characterModel.level;
        SetAnimator(characterObject.GetComponent<Animator>());

        this.HasInitialized = true;
    }

    public void SetAnimator(Animator animator) {
        _characterAnimator.SetAnimator(animator);
    }

    public void Sit() {
        _characterAnimator.Sit();
    }

    public void StandUp() {
        _characterAnimator.StandUp();
    }

    public void OnSelected() {
        TargetIndicator.instance.SetPosition(transform, TargetIndicator.Type.CharacterSelection);
        SlotHighlighter.instance.SetPosition(transform);
        StandUp();
        onSelected?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnSelected();
    }
}
