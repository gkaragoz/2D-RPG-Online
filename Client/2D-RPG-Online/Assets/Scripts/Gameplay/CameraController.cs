using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public enum SceneBehaviour {
        Gameplay,
        CharacterSelection
    }

    [Header("Initialization")]
    [SerializeField]
    private PhysicsRaycaster _physicsRaycaster;
    [SerializeField]
    private Cinemachine.CinemachineBrain _cinemachineBrain;
    [SerializeField]
    private string _zoomingAxis = "Mouse ScrollWheel";

    [Header("Settings")]
    [SerializeField]
    private bool _blockProcess;
    [SerializeField]
    private bool _useScrollwheelZooming = true;
    [SerializeField]
    private bool _smoothFollow = true;
    [SerializeField]
    private float _smoothSpeed = 0.125f;
    [SerializeField]
    private Vector3 _offset;

    [SerializeField]
    private float _maxHeight = 10f;
    [SerializeField]
    private float _minHeight = 5f;
    [SerializeField]
    private float _heightDampening = 5f;
    [SerializeField]
    private float _scrollWheelZoomingSensitivity = 60f;
    
    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Transform _target;
    [SerializeField]
    [Utils.ReadOnly]
    private float _zoomPos = 0;

    private float ScrollWheel {
        get { return Input.GetAxis(_zoomingAxis); }
    }

    private void Start() {
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;
    }

    private void LateUpdate() {
        if (_blockProcess) {
            return;
        }
        if (_target == null) {
            return;
        }
        if (_useScrollwheelZooming) {
            HeightCalculation();
        }

        if (_smoothFollow) {
            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = smoothedPosition;
        } else {
            Vector3 desiredPosition = _target.position + _offset;
            transform.position = desiredPosition;
        }
    }

    public void SetBehaviour(SceneBehaviour behaviour) {
        switch (behaviour) {
            case SceneBehaviour.Gameplay:
                _cinemachineBrain.enabled = false;
                _physicsRaycaster.enabled = false;
                transform.localRotation = Quaternion.Euler(new Vector3(45, 0f, 0));
                transform.localPosition = new Vector3(0, 8, -7);
                _blockProcess = false;
                break;
            case SceneBehaviour.CharacterSelection:
                _physicsRaycaster.enabled = true;
                _cinemachineBrain.enabled = true;
                _blockProcess = true;
                break;
            default:
                break;
        }
    }

    public void ApplyRotation(int amount) {
        transform.Rotate(Vector3.right * amount);
    }

    public void SetTarget(Transform target) {
        this._target = target;
    }

    private void OnLoadingCompleted() {
        if (SceneController.instance.GetActiveScene().name == "CharacterSelection") {
            Camera.main.GetComponent<CameraController>().SetBehaviour(CameraController.SceneBehaviour.CharacterSelection);
        } else if (SceneController.instance.GetActiveScene().name == "Gameplay") {
            Camera.main.GetComponent<CameraController>().SetBehaviour(CameraController.SceneBehaviour.Gameplay);
        }
    }

    private void HeightCalculation() {
        //if (useScrollwheelZooming)
        //    _zoomPos -= ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;

        //_zoomPos = Mathf.Clamp01(_zoomPos);

        //float targetHeight = Mathf.Lerp(minHeight, maxHeight, _zoomPos);

        //Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetHeight, Time.deltaTime * heightDampening);
    }

}
