using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMotor : MonoBehaviour {

    [Header("Initialization")]
    public float speed = 3f;

    [SerializeField]
    private Transform _groundInTransform;
    [SerializeField]
    private Transform _groundOutTransform;

    private Rigidbody2D _rb2D;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public void JumpToPosition(Vector2 target) {
        StartCoroutine(IJumpToPosition(target));
    }

    private IEnumerator IJumpToPosition(Vector2 target) {
        float t = 0;
        Vector2 start = transform.position;
        Vector2 endPosition = start + target;

        while (t <= 1) {
            t += Time.fixedDeltaTime / speed;
            _rb2D.MovePosition(Vector2.Lerp(start, endPosition, t));

            yield return null;
        }

        StopAllCoroutines();
    }

}
