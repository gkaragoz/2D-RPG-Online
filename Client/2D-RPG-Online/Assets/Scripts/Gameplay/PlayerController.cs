using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IAttackable {

    public class PositionEntry {
        public Vector3 vector3;
        public double updateTime;
        public int inputSequenceID;

        public PositionEntry(double updateTime, Vector3 vector3, int inputSequenceID) {
            this.updateTime = updateTime;
            this.vector3 = vector3;
            this.inputSequenceID = inputSequenceID;
        }
    }

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

    public Vector3 CurrentInput { get; private set; }
    public List<SPlayerInput> PlayerInputs { get { return _playerInputs; } }
    public int Oid { get { return _playerData.Oid; } }
    public bool IsMe { get { return _isMe; } }

    public List<PositionEntry> PositionBuffer { get { return _positionBuffer; } set { _positionBuffer = value; } }

    public int LastProcessedInputSequenceID { get { return _lastProcessedInputSequenceID; } set { _lastProcessedInputSequenceID = value; } }

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
    private CharacterController _characterController;
    private PlayerObject _playerData;
    private CharacterStats _characterStats;
    private Joystick _joystick;
    private Button _btnAttack;

    private List<PositionEntry> _positionBuffer = new List<PositionEntry>();
    private int _lastProcessedInputSequenceID;
    private List<SPlayerInput> _playerInputs = new List<SPlayerInput>();
    private int _nonAckInputIndex = 0;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _characterStats = GetComponent<CharacterStats>();

        CreateHUD();
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
                ShiftServerData data = new ShiftServerData();

                data.PlayerInput = new SPlayerInput();
                data.PlayerInput.SequenceID = _nonAckInputIndex++;

                data.PlayerInput.PosX = CurrentInput.x;
                data.PlayerInput.PosZ = CurrentInput.z;
                data.PlayerInput.PressTime = Time.fixedDeltaTime;

                NetworkManager.mss.SendMessage(MSPlayerEvent.Move, data);

                PlayerInputs.Add(data.PlayerInput);
            }

            if (HasInput) {
                Move();
            } else {
                Stop();
            }
        }

        //DEBUG PURPOSES
        UpdatePlayerInputsUI();
    }

    public void Initialize(PlayerObject playerData) {
        this._playerData = playerData;
        this._characterStats.Initialize(playerData);

        if (_playerData.Name == AccountManager.instance.SelectedCharacterName) {
            _isMe = true;

            Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
        } else {
            _isMe = false;
            HideControllers();
        }
    }

    public void AddPositionToBuffer(double timestamp, Vector3 position, int inputSequenceID) {
        _positionBuffer.Add(new PositionEntry(timestamp, position, inputSequenceID));
    }

    public Vector2 GetVectorByInput(int index) {
        return new Vector2(PlayerInputs[index].PosX, PlayerInputs[index].PosY);
    }

    public void ClearPlayerInputs() {
        _playerInputs = new List<SPlayerInput>();
    }

    public void RemoveRange(int index, int count) {
        _playerInputs.RemoveRange(index, count);
    }

    public int GetSequenceID(int index) {
        return PlayerInputs[index].SequenceID;
    }

    public void SetJoystick(FixedJoystick joystick) {
        this._joystick = joystick;
    }

    public void ShowControllers() {
        CharacterController.ShowControllers();
    }

    public void HideControllers() {
        CharacterController.HideControllers();
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

    public void Move() {
        CharacterController.Move(CurrentInput);
    }

    public void Move(Vector3 input) {
        CharacterController.Move(input);
    }

    public void Stop() {
        CharacterController.Stop();
    }

    public void Rotate() {
        CharacterController.Rotate(CurrentInput);
    }

    public void ToNewPosition(Vector3 newPosition) {
        CharacterController.ToNewPosition(newPosition);
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
        _txtNonAckPlayerInputs.text = PlayerInputs.Count.ToString();
    }

}
