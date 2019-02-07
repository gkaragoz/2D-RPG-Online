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
    public float scrollWheelZoomingSensitivity = 60f;

    private float _zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

    public bool useScrollwheelZooming = true;
    public string zoomingAxis = "Mouse ScrollWheel";

    private float ScrollWheel {
        get { return Input.GetAxis(zoomingAxis); }
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate() {
        if (useScrollwheelZooming)
            HeightCalculation();

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

    private void HeightCalculation() {
        //if (useScrollwheelZooming)
        //    _zoomPos -= ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;

        //_zoomPos = Mathf.Clamp01(_zoomPos);

        //float targetHeight = Mathf.Lerp(minHeight, maxHeight, _zoomPos);

        //Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetHeight, Time.deltaTime * heightDampening);
    }

    public void ApplyRotation(int amount) {
        transform.Rotate(Vector3.right * amount);
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }
}
