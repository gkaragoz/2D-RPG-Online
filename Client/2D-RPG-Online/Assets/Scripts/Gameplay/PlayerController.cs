using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Settings")]
    [SerializeField]
    private bool _controllerInput;
    [SerializeField]
    private bool _joystickInput;

    private CharacterController _characterController;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {
        LastInput = CurrentInput;

        if (_controllerInput) {
            _xInput = Input.GetAxisRaw("Horizontal");
            _yInput = Input.GetAxisRaw("Vertical");
        }

        if (_joystickInput && (_xInput == 0 || _yInput == 0)) {
            _xInput = _joystick.Horizontal;
            _yInput = _joystick.Vertical;
        }

        CurrentInput = new Vector2(_xInput, _yInput);

        if (HasInput) { 
            Move();
        } else {
            Stop();
        }

        _xInput = 0;
        _yInput = 0;
    }

    private void Update() {
        if (Input.GetButton("Fire1")) {
            Attack();
        }
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

    public void Stop() {
        _characterController.Stop();
    }

}
