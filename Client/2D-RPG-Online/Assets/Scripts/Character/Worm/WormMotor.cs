using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    private Rigidbody2D _rb2D;
    private Vector2 _target;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public void JumpToPosition(Vector2 position) {
        Vector2 start = transform.position;
        _target = start + position;
    }

    private void FixedUpdate() {
        if (_target != Vector2.zero) {
            float step = speed * Time.fixedDeltaTime;

            transform.position = Vector3.MoveTowards(transform.position, _target, step);
        }
    }

}
