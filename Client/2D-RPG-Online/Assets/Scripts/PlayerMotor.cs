using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction) {
        Vector2 currentPosition = transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));
    }

}
