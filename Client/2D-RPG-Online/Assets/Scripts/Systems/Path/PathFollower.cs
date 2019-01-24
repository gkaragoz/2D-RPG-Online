using UnityEditor;
using UnityEngine;

public class PathFollower : MonoBehaviour {

    public delegate void PathCompleted();
    public PathCompleted onPathCompleted;

    [Header("Initialization")]
    public PathEditor pathEditor;
    public float movementSpeed;
    public float stoppingDistance = 0.2f;
    public float rotationSpeed = 3.5f;
    public bool rotate = false;
    public bool runOnStart = false;
    public bool mixPath = false;
    public bool repeatForever = false;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _currentPathPointIndex = 0;
    [SerializeField]
    [Utils.ReadOnly]
    private bool isRunning = false;

    public bool HasPathCompleted { get { return _currentPathPointIndex >= pathEditor.allPaths.Count ? true : false; } }

    private Vector3 _desiredPointPosition;
    private Vector3 _currentPosition;

    private void Start() {
        if (runOnStart) {
            transform.position = pathEditor.GetRandomPathPoint();
            
            Run();
        }
    }

    private void Update() {
        if (!isRunning)
            return;

        if (!HasPathCompleted) {
            _currentPosition = transform.position;
            _desiredPointPosition = pathEditor.GetPathPoint(_currentPathPointIndex);

            Move();
            Rotate();

            if (HasReachedToPoint()) {
                if (mixPath) {
                    SetNextPointAsARandom();
                } else {
                    SetNextPoint();
                }
            }

            if (HasCompletedPath()) {
                OnPathCompleted();
            }
        }
    }

    public void Run() {
        if (mixPath) {
            SetNextPointAsARandom();
        }

        isRunning = true;
    }

    public void Stop() {
        isRunning = false;
    }

    private void Move() {
        transform.position = Vector3.MoveTowards(_currentPosition, _desiredPointPosition, movementSpeed * Time.deltaTime);
    }

    private void Rotate() {
        if (rotate) {
            Vector3 rotationVector = _desiredPointPosition - _currentPosition;

            if (rotationVector == Vector3.zero) {
                return;
            }

            Quaternion desiredRotation = Quaternion.LookRotation(_desiredPointPosition - _currentPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private float GetDistanceToDestination() {
        return Vector3.Distance(_desiredPointPosition, transform.position);
    }
    
    private void SetNextPoint() {
        _currentPathPointIndex++;
    }

    private void SetNextPointAsARandom() {
        _currentPathPointIndex = pathEditor.GetRandomPathIndex();
    }

    private bool HasReachedToPoint() {
        return GetDistanceToDestination() <= stoppingDistance ? true : false;
    }

    private bool HasCompletedPath() {
        return _currentPathPointIndex >= pathEditor.allPaths.Count ? true : false;
    }

    private void OnPathCompleted() {
        Stop();
        _currentPathPointIndex = 0;

        if (repeatForever) {
            Run();
        }

        if (onPathCompleted != null) {
            onPathCompleted.Invoke();
        }
    }

}

[CustomEditor(typeof(PathFollower))]
public class PathFollowerDrawer : Editor {

    public override void OnInspectorGUI() {
        PathFollower script = (PathFollower)target;

        GUI.backgroundColor = Color.green;

        base.OnInspectorGUI();
    }

}