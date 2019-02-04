using ManaShiftServer.Data.Utils;
using TMPro;
using UnityEngine;
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

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _zInput;
    [SerializeField]
    private bool _isOfflineMode;
    [SerializeField]
    private bool _isControllerActive;
    [SerializeField]
    private GameObject _HUDPrefab;

    private bool _isMe;
    private CharacterController _characterController;
    private Joystick _joystick;
    private Button _btnAttack;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start() {
        if (_isOfflineMode) {
            this.Initialize(null);
        }
    }

    private void FixedUpdate() {
        if (_isMe || _isOfflineMode) {
            if (_isControllerActive) {
                _xInput = Input.GetAxis("Horizontal");
                _zInput = Input.GetAxis("Vertical");
            } else {
                if (_joystick != null) {
                    _xInput = _joystick.Horizontal;
                    _zInput = _joystick.Vertical;
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

        this._characterController.Initialize(networkIdentifier, this, _isOfflineMode);

        if (!_isOfflineMode) {
            if (_networkEntity.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
                _isMe = true;

                CreateHUD();

                Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
            } else {
                _isMe = false;
            }
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

    private void CreateHUD() {
        GameObject HUDObject = Instantiate(_HUDPrefab, transform);
        _joystick = HUDObject.GetComponentInChildren<FixedJoystick>();
        _btnAttack = HUDObject.transform.Find("btnAttack").GetComponent<Button>();

        _btnAttack.onClick.AddListener(Attack);
    }

    /// <summary>
    /// DEBUG
    /// </summary>
    /// 
    [SerializeField]
    private TextMeshProUGUI _txtNonAckPlayerInputs;

    private void UpdatePlayerInputsUI() {
        if (_networkEntity != null) {
            _txtNonAckPlayerInputs.text = _networkEntity.PlayerInputs.Count.ToString();
        }
    }

}
