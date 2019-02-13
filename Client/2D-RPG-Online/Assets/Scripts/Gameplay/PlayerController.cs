using ManaShiftServer.Data.Utils;
using TMPro;
using UnityEngine;

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

    public Vector3 CurrentInput { get; set; }

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

        _characterController.onAttack += OnAttack;
        _characterController.onMove += OnMove;
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
                if (Utils.IsPointerOverUIObject()) {
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
                if (Utils.IsPointerOverUIObject()) {
                    return;
                }

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    Debug.Log(hit.transform.gameObject.name);
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

            AutoAttack();

            CurrentInput = new Vector3(_xInput, 0, _zInput);

            if (HasInput) {
                Move();
            } else {
                Stop();
            }
        } else {
            //Move others.
            if (Utils.IsValid(this.NetworkEntity.NetworkObject.PositionX, this.NetworkEntity.NetworkObject.PositionY, this.NetworkEntity.NetworkObject.PositionZ)) {
                Vector3 newPosition = new Vector3(this.NetworkEntity.NetworkObject.PositionX.ToFloat(), this.NetworkEntity.NetworkObject.PositionY.ToFloat(), this.NetworkEntity.NetworkObject.PositionZ.ToFloat());

                if (newPosition != this.transform.position) {
                    this.Move(newPosition);
                } else {
                    this.Stop();
                }
            } else {
                this.Stop();
            }
        }

        //DEBUG PURPOSES
        UpdatePlayerInputsUI();
    }

    public void Initialize(NetworkIdentifier networkIdentifier) {
        _networkEntity = new NetworkEntity(networkIdentifier);

        gameObject.name = _networkEntity.NetworkObject.PlayerData.Name + "(" + _networkEntity.NetworkObject.Id + ")";

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

    public void SetJoystick(FixedJoystick joystick) {
        this._joystick = joystick;
    }

    public void SelectTarget(LivingEntity livingEntity) {
        _characterController.SelectTarget(livingEntity);

        if (_isMe) {
            TargetIndicator.instance.SetPosition(livingEntity.transform, TargetIndicator.Type.Enemy);
        }
    }

    public void DeselectTarget() {
        _characterController.DeselectTarget();

        if (_isMe) {
            TargetIndicator.instance.Hide();
        }
    }

    public void TakeDamage(int damage) {
        _characterController.TakeDamage(damage);
    }

    public void Die() {
        _characterController.Die();
    }

    public void Attack() {
        _characterController.Attack();
    }

    public void AutoAttack() {
        _characterController.AutoAttack();
    }

    public void Move() {
        if (_isMe) {
            _characterController.MoveToInput(CurrentInput);
        }
    }

    public void Move(Vector3 newPosition) {
        _characterController.MoveToPosition(newPosition);
    }

    public void Stop() {
        _characterController.Stop();
    }

    public void Rotate() {
        _characterController.Rotate(CurrentInput);
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }
    
    private void OnMove() {
        if (_isMe) {
            this.NetworkEntity.SendMovementInputData(CurrentInput);
        }
    }

    private void OnAttack(int targetID) {
        if (_isMe) {
            this.NetworkEntity.SendAttackInputData(targetID);
        }
    }

    private void UpdatePlayerInputsUI() {
        if (_networkEntity != null) {
            _txtNonAckPlayerInputs.text = _networkEntity.PlayerInputs.Count.ToString();
        }
    }

}
