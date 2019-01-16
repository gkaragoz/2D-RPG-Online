using System.Collections;
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
    public Vector2 LastInput { get; private set; }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private Button _btnAttack;

    private CharacterController _characterController;
    private PlayerHUD _playerHUD;
    private RoomPlayerInfo _playerInfo;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
        _playerHUD = GetComponent<PlayerHUD>();
    }

    private void FixedUpdate() {
        LastInput = CurrentInput;

        _xInput = _joystick.Horizontal;
        _yInput = _joystick.Vertical;

        CurrentInput = new Vector2(_xInput, _yInput);

        if (HasInput && NetworkManager.mss != null) {
            ShiftServerData data = new ShiftServerData();

            data.PlayerInput = new SPlayerInput();
            data.PlayerInput.PosX = CurrentInput.x;
            data.PlayerInput.PosY = CurrentInput.y;

            NetworkManager.mss.SendMessage(MSPlayerEvent.Move, data);
        }

        if (HasInput) {
            Move();
        } else {
            Stop();
        }
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

}
