using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public delegate void AttackEvent();
    public event AttackEvent onAttacking;

    public float speed = 3f;

    public bool HasInput {
        get {
            return (xInput != 0 || yInput != 0) ? true : false;
        }
    }

    public Vector2 CurrentDirection { get; private set; }
    public bool IsAttacking { get; private set; }

    private float xInput, yInput;
    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        CurrentDirection = new Vector2(xInput, yInput);

        if (HasInput && !IsAttacking) {
            Move(CurrentDirection);
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space) && !IsAttacking) {
            Attack();
        }
    }

    public void Attack() {
        IsAttacking = true;

        if (onAttacking != null) {
            onAttacking.Invoke();
        }
    }

    public void OnHit() {
        Debug.Log("On Hit.");
        IsAttacking = false;
    }

    public void Move(Vector2 direction) {
        Vector2 currentPosition = transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));
    }

}
