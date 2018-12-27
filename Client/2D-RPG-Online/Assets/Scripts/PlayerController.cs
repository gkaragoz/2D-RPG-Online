using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 3f;

    public bool HasInput {
        get {
            return (xInput != 0 || yInput != 0) ? true : false;
        }
    }

    public Vector2 currentDirection { get; private set; }

    private float xInput, yInput;
    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        currentDirection = new Vector2(xInput, yInput);

        if (HasInput) {
            Move(currentDirection);
        }
    }

    public void Move(Vector2 direction) {
        Vector2 currentPosition = transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));
    }

}
