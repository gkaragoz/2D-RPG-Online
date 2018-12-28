using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour {

    public bool HasInput {
        get {
            return (_xInput != 0 || _yInput != 0) ? true : false;
        }
    }

    public Vector2 CurrentDirection {
        get {
            return new Vector2(_xInput, _yInput);
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    private PlayerMotor _playerMotor;
    private PlayerAttack _playerAttack;

    private void Start() {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    private void FixedUpdate() {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if (HasInput && !_playerAttack.IsAttacking) {
            _playerMotor.Move(CurrentDirection);
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space) && !_playerAttack.IsAttacking && _playerAttack.CanAttack) {
            _playerAttack.Attack();
        }
    }

}
