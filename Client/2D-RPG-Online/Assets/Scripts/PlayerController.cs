using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public bool HasInput {
        get {
            return (_xInput != 0 || _yInput != 0) ? true : false;
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    private CharacterController _characterController;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if (HasInput) { 
            Move();
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Attack();
        }
    }

    public void Attack() {
        _characterController.Attack();
    }

    public void Move() {
        _characterController.Move(new Vector2(_xInput, _yInput));
    }

}
