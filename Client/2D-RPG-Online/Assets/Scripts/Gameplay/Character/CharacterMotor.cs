using System;
using UnityEngine;

public class CharacterMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    public bool IsMoving {
        get {
            return _rb.velocity.magnitude > 0 ? true : false;
        }
    }

    private Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
    }

    public void SetMovementSpeed(float speed) {
        this.speed = speed;
    }

    public void Move(Vector3 direction) {
        Vector3 currentPosition = _rb.transform.position;

        _rb.transform.rotation = Quaternion.LookRotation(direction);
        _rb.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("footstep");
        }
    }

    public void Rotate(Vector3 direction) {
        _rb.transform.rotation = Quaternion.Euler(transform.position - direction);
    }

}
