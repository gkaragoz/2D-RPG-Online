using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    public bool IsMoving {
        get {
            return _rb2D.velocity.magnitude > 0 ? true : false;
        }
    }

    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction) {
        Vector2 currentPosition = _rb2D.transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));

        AudioManager.instance.Play("footstep");
    }

}
