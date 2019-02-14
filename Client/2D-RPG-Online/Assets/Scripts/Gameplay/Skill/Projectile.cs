using UnityEngine;

public class Projectile : MonoBehaviour {

    private float _offsetY = 1.0f;
    private float _speed = 5f;
    private Transform _target;
    private bool _isRunning = false;

    private Vector3 TargetPosition {
        get {
            return new Vector3(_target.position.x, _offsetY, _target.position.z);
        }
    }

    private float TargetDistance {
        get {
            Vector3 myPosition = transform.position;
            Vector3 targetPosition = new Vector3(_target.position.x, _offsetY, _target.position.z);

            return Vector3.Distance(myPosition, targetPosition);
        }
    }

    private void Start() {
        gameObject.AddComponent<BoxCollider>().size = Vector3.one;
    }

    private void Update() {
        if (_isRunning) {
            if (_target != null) {
                if (TargetDistance >= 0.1f) {
                    Move();
                } else {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Run() {
        _isRunning = true;
    }

    public void SetTarget(Transform target) {
        this._target = target;
    }

    private void Move() {
        Vector3 direction = TargetPosition - transform.position;
        transform.parent.Translate(direction.normalized * _speed * Time.deltaTime);
    }

}
