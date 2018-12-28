using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public delegate void AttackEvent();
    public event AttackEvent onAttacking;

    [Header("Initialization")]
    public float speed = 3f;
    public float attackSpeed = 1f;

    public bool HasInput {
        get {
            return (_xInput != 0 || _yInput != 0) ? true : false;
        }
    }

    public Vector2 CurrentDirection { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool CanAttack {
        get {
            return Time.time > _nextAttackTime;
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    [SerializeField]
    [Utils.ReadOnly]
    private float _nextAttackTime;
    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();

        _nextAttackTime = 2f;
    }

    private void FixedUpdate() {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        CurrentDirection = new Vector2(_xInput, _yInput);

        if (HasInput && !IsAttacking) {
            Move(CurrentDirection);
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space) && !IsAttacking && CanAttack) {
            Attack();
        }
    }

    public void Attack() {
        _nextAttackTime = Time.time + attackSpeed;

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
