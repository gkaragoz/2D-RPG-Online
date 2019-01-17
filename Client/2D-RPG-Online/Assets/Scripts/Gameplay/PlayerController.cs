using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public class PositionEntry {
        public Vector2 vector2;
        public double updateTime;

        public PositionEntry(double updateTime, Vector2 vector2) {
            this.updateTime = updateTime;
            this.vector2 = vector2;
        }
    }

    public bool HasInput {
        get {
            return (CurrentInput != Vector2.zero) ? true : false;
        }
    }

    public Vector2 CurrentInput { get; private set; }
    public List<SPlayerInput> PlayerInputs { get { return _playerInputs; } }
    public int Oid { get { return _playerData.Oid; } }
    public bool IsMe { get { return _isMe; } }

    public List<PositionEntry> PositionBuffer { get { return _positionBuffer; } set { _positionBuffer = value; } }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private Button _btnAttack;

    private bool _isMe;
    private CharacterController _characterController;
    private PlayerHUD _playerHUD;
    private PlayerObject _playerData;

    private List<PositionEntry> _positionBuffer = new List<PositionEntry>();

    private List<SPlayerInput> _playerInputs = new List<SPlayerInput>();
    private int _nonAckInputIndex = 0;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerHUD = GetComponent<PlayerHUD>();
    }

    private void FixedUpdate() {
        if (_isMe) {
            _xInput = _joystick.Horizontal;
            _yInput = _joystick.Vertical;

            CurrentInput = new Vector2(_xInput, _yInput);

            if (HasInput && NetworkManager.mss != null) {
                ShiftServerData data = new ShiftServerData();

                data.PlayerInput = new SPlayerInput();
                data.PlayerInput.SequenceID = _nonAckInputIndex++;

                data.PlayerInput.PosX = CurrentInput.x;
                data.PlayerInput.PosY = CurrentInput.y;

                NetworkManager.mss.SendMessage(MSPlayerEvent.Move, data);

                PlayerInputs.Add(data.PlayerInput);
            }

            if (HasInput) {
                Move();
            } else {
                Stop();
            }
        }

        UpdateHUD();
    }

    private void Update() {
        if (Input.GetButton("Fire1")) {
            Attack();
        }
    }

    public void Initialize(PlayerObject playerData) {
        this._playerData = playerData;

        _playerHUD.SetName(_playerData.Name);

        InitializeCharacter(_playerData);

        if (_playerData.Name == AccountManager.instance.SelectedCharacterName) {
            _isMe = true;
        } else {
            _isMe = false;
            _playerHUD.Hide();
        }
    }

    public void AddPositionToBuffer(double timestamp, Vector2 position) {

        _positionBuffer.Add(new PositionEntry(timestamp, position));
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

    public void InitializeCharacter(PlayerObject playerData) {
        _characterController.Initiailize(playerData);
    }

    public void Attack() {
        _characterController.Attack();
    }

    public void Move() {
        _characterController.Move(CurrentInput);
    }

    public void Move(Vector2 input) {
        _characterController.Move(input);
    }

    public void ToNewPosition(Vector2 newPosition) {
        _characterController.ToNewPosition(newPosition);
    }

    public void Stop() {
        _characterController.Stop();
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    private void UpdateHUD() {
        _playerHUD.UpdateHUD(PlayerInputs.Count);
    }

}
