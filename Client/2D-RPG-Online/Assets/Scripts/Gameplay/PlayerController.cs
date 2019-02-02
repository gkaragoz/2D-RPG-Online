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

    public NetworkEntity NetworkIdentifier { get { return _networkIdentifier; } }
    public Vector3 CurrentInput { get; private set; }
    public CharacterController CharacterController { get { return _characterController; } }

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
    private NetworkEntity _networkIdentifier;
    private CharacterController _characterController;
    private Joystick _joystick;
    private Button _btnAttack;

    public override void Awake() {
        base.Awake();

        _characterController = GetComponent<CharacterController>();
    }

    private void Start() {
        if (_isOfflineMode) {
            this.CharacterController.InitializeOffline(this);
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

            if (!_isOfflineMode && HasInput && NetworkManager.mss != null) {
                _networkIdentifier.SendInputData(CurrentInput);
            }

            if (HasInput) {
                MoveByInput();
            } else {
                Stop();
            }
        }

        //DEBUG PURPOSES
        UpdatePlayerInputsUI();
    }

    public void Initialize(NetworkIdentifier networkData) {
        _networkIdentifier = new NetworkEntity(networkData);

        this.CharacterController.Initialize(networkData, this);

        if (_networkIdentifier.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
            _isMe = true;

            CreateHUD();

            Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
        } else {
            _isMe = false;
        }
    }

    public void Render(NetworkEntity networkData) {
        _networkIdentifier = networkData;

        if (Utils.IsValid(_networkIdentifier.NetworkObject.PositionX, _networkIdentifier.NetworkObject.PositionY, _networkIdentifier.NetworkObject.PositionZ)) {
            Vector3 newPosition = new Vector3(_networkIdentifier.NetworkObject.PositionX.ToFloat(), _networkIdentifier.NetworkObject.PositionY.ToFloat(), _networkIdentifier.NetworkObject.PositionZ.ToFloat());

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
        CharacterController.TakeDamage(damage);
    }

    public override void OnDeath() {
        CharacterController.OnDeath();
    }

    public override void Attack() {
        CharacterController.Attack();
    }

    public override void MoveByInput() {
        CharacterController.MoveToInput(CurrentInput);
    }

    public override void MoveToPosition(Vector3 position) {
        CharacterController.MoveToPosition(position);
    }

    public override void Stop() {
        CharacterController.Stop();
    }

    public override void Rotate() {
        CharacterController.Rotate(CurrentInput);
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
        if (_networkIdentifier != null) {
            _txtNonAckPlayerInputs.text = _networkIdentifier.PlayerInputs.Count.ToString();
        }
    }

}
