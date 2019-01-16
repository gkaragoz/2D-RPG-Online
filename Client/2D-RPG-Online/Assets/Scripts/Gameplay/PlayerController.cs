using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public bool HasInput {
        get {
            return (CurrentInput != Vector2.zero) ? true : false;
        }
    }

    public Vector2 CurrentInput { get; private set; }
    public List<SPlayerInput> PlayerInputs { get { return _playerInputs; } }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private Button _btnAttack;
    [SerializeField]
    private GameObject _shadowObject;

    private CharacterController _characterController;
    private PlayerHUD _playerHUD;
    private RoomPlayerInfo _playerInfo;

    private List<SPlayerInput> _playerInputs = new List<SPlayerInput>();
    private int _nonAckInputIndex = 0;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerHUD = GetComponent<PlayerHUD>();
    }

    private void FixedUpdate() {
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

        UpdateHUD();
    }

    private void Update() {
        if (Input.GetButton("Fire1")) {
            Attack();
        }
    }

    public void Initialize(RoomPlayerInfo playerInfo) {
        this._playerInfo = playerInfo;

        _playerHUD.SetName(this._playerInfo.Username);
    }

    public void SetShadowPosition(Vector2 position) {
        _shadowObject.transform.position = position;

        if (_shadowObject.transform.position == this.transform.position) {
            _shadowObject.gameObject.SetActive(false);
        } else {
            _shadowObject.gameObject.SetActive(true);
        }
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

    public void Attack() {
        _characterController.Attack();
    }

    public void Move() {
        _characterController.Move(CurrentInput);
    }

    public void Move(Vector2 input) {
        _characterController.Move(input);
    }

    public void Stop() {
        _characterController.Stop();
    }

    private void UpdateHUD() {
        _playerHUD.Update(PlayerInputs.Count);
    }

}
