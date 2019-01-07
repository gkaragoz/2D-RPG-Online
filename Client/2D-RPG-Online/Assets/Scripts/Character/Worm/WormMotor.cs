using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponentInChildren<Rigidbody2D>();
    }

    public void Jump(Vector2 direction) {
        Vector2 currentPosition = _rb2D.transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));
    }

}
