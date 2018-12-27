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

    private float xInput, yInput;
    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (HasInput) {
            Move();
        }
    }

    public void Move() {
        Vector2 currentPosition = transform.position;
        _rb2D.MovePosition(new Vector2(currentPosition.x + (xInput * speed * Time.fixedDeltaTime), 
                                      currentPosition.y + (yInput * speed * Time.fixedDeltaTime)));
    }

}
