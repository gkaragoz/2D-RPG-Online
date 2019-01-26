using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public bool HasInput {
        get {
            return (CurrentInput != Vector3.zero) ? true : false;
        }
    }

    public int AttackDamage {
        get {
            return _characterStats.GetAttackDamage();
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
    private CharacterStats _characterStats;
    private Joystick _joystick;
    private Button _btnAttack;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void Start() {
        _characterStats.characterDefinition.onDeath += OnDeath;
    }

    private void FixedUpdate() {
        if (_isMe || _isOfflineMode) {
            if (_isControllerActive) {
                _xInput = Input.GetAxis("Horizontal");
                _zInput = Input.GetAxis("Vertical");
            } else {
                _xInput = _joystick.Horizontal;
                _zInput = _joystick.Vertical;
            }

            if (Input.GetButtonDown("Fire1")) {
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

    public void Initialize(NetworkIdentifier networkObject) {
        _networkIdentifier = new NetworkEntity(networkObject);

        this._characterStats.Initialize(networkObject);

        if (_networkIdentifier.NetworkObject.PlayerData.Name == AccountManager.instance.SelectedCharacterName) {
            _isMe = true;

            CreateHUD();

            Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
        } else {
            _isMe = false;
        }
    }

    public void SetJoystick(FixedJoystick joystick) {
        this._joystick = joystick;
    }

    public void TakeDamage(int damage) {
        _characterStats.TakeDamage(damage);
        CharacterController.TakeDamage();
    }

    public void OnDeath() {
        CharacterController.OnDeath();
    }

    public void Attack() {
        CharacterController.Attack();
    }

    public void MoveByInput() {
        CharacterController.MoveToInput(CurrentInput);
    }

    public void MoveToPosition(Vector3 position) {
        if (position == Vector3.zero) {
            Stop();
        } else {
            CharacterController.MoveToPosition(position);
        }
    }

    public void Stop() {
        CharacterController.Stop();
    }

    public void Rotate() {
        CharacterController.Rotate(CurrentInput);
    }

    public void Destroy() {
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
        _txtNonAckPlayerInputs.text = _networkIdentifier.PlayerInputs.Count.ToString();
    }

}
