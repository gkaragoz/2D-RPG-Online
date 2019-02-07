using ManaShiftServer.Data.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : LivingEntity {

    public bool HasInput {
        get {
            return (CurrentInput != Vector3.zero) ? true : false;
        }
    }

    public int AttackDamage {
        get {
            return _characterController.AttackDamage;
        }
    }

    public Vector3 CurrentInput { get; private set; }

    /// <summary>
    /// DEBUG
    /// </summary>
    /// 
    [SerializeField]
    private TextMeshProUGUI _txtNonAckPlayerInputs;
    [SerializeField]
    private bool _isControllerActive;
    [SerializeField]
    private GameObject _HUDObject;
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private LayerMask selectables;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private bool _isMe;
    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _zInput;
    private CharacterController _characterController;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
        if (_isMe) {
            if (_isControllerActive) {
                _xInput = Input.GetAxis("Horizontal");
                _zInput = Input.GetAxis("Vertical");
            } else {
                if (_joystick != null) {
                    _xInput = _joystick.Horizontal;
                    _zInput = _joystick.Vertical;
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    if (selectables == (selectables | (1 << hit.collider.gameObject.layer))) {
                        SelectTarget(hit.collider.gameObject.GetComponent<LivingEntity>());
                    } else {
                        DeselectTarget();
                    }
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    if (selectables == (selectables | (1 << hit.collider.gameObject.layer))) {
                        SelectTarget(hit.collider.gameObject.GetComponent<LivingEntity>());
                    } else {
                        DeselectTarget();
                    }
                }
            }

            if (Input.GetButton("Fire1")) {
                Attack();
            }

            CurrentInput = new Vector3(_xInput, 0, _zInput);

            if (HasInput) {
                MoveByInput();
            } else {
                Stop();
            }
        }

        //DEBUG PURPOSES
        UpdatePlayerInputsUI();
    }

    public void Initialize(NetworkIdentifier networkIdentifier) {
        _networkEntity = new NetworkEntity(networkIdentifier);

        this._playerClass = _networkEntity.NetworkObject.PlayerData.PClass;

        switch (this._playerClass) {
            case PlayerClass.Warrior:
                _characterObject = Instantiate(_warriorPrefab, this.transform);
                break;
            case PlayerClass.Archer:
                _characterObject = Instantiate(_archerPrefab, this.transform);
                break;
            case PlayerClass.Mage:
                _characterObject = Instantiate(_magePrefab, this.transform);
                break;
            case PlayerClass.Priest:
                _characterObject = Instantiate(_priestPrefab, this.transform);
                break;
            default:
                Debug.LogError("CLASS NOT FOUND!");
                break;
        }

        this._characterController.Initialize(networkIdentifier, this);

        if (_networkEntity.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
            _isMe = true;

            Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
        } else {
            _isMe = false;

            Destroy(_HUDObject);
        }
    }

    public void Render(NetworkEntity networkData) {
        _networkEntity = networkData;

        if (Utils.IsValid(_networkEntity.NetworkObject.PositionX, _networkEntity.NetworkObject.PositionY, _networkEntity.NetworkObject.PositionZ)) {
            Vector3 newPosition = new Vector3(_networkEntity.NetworkObject.PositionX.ToFloat(), _networkEntity.NetworkObject.PositionY.ToFloat(), _networkEntity.NetworkObject.PositionZ.ToFloat());

            if (newPosition != transform.position) {
                MoveToPosition(newPosition);
            } else {
                Stop();
            }
        } else {
            Stop();
        }
    }

    public void SetJoystick(FixedJoystick joystick) {
        this._joystick = joystick;
    }

    public void SelectTarget(LivingEntity livingEntity) {
        _characterController.SelectTarget(livingEntity);
    }

    public void DeselectTarget() {
        _characterController.DeselectTarget();
    }

    public override void TakeDamage(int damage) {
        _characterController.TakeDamage(damage);
    }

    public override void Die() {
        _characterController.Die();
    }

    public override void Attack() {
        _characterController.Attack();
    }

    public override void AttackAnimation(Vector3 direction) {
        _characterController.AttackAnimation(direction);
    }

    public override void MoveByInput() {
        _characterController.MoveToInput(CurrentInput);
    }

    public override void MoveToPosition(Vector3 position) {
        _characterController.MoveToPosition(position);
    }

    public override void Stop() {
        _characterController.Stop();
    }

    public override void Rotate() {
        _characterController.Rotate(CurrentInput);
    }

    public override void Destroy() {
        Destroy(this.gameObject);
    }

    private void UpdatePlayerInputsUI() {
        if (_networkEntity != null) {
            _txtNonAckPlayerInputs.text = _networkEntity.PlayerInputs.Count.ToString();
        }
    }

}
