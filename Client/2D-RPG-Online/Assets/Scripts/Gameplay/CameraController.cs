using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;

    public bool smoothFollow = true;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float maxHeight = 10f;
    public float minHeight = 5f;
    public float heightDampening = 5f;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate() {
        if (target == null)
            return;

        if (smoothFollow) {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        } else {
            Vector3 desiredPosition = target.position + offset;
            transform.position = desiredPosition;
        }
    }

    public void ApplyRotation(int amount) {
        transform.Rotate(Vector3.right * amount);
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }
}
