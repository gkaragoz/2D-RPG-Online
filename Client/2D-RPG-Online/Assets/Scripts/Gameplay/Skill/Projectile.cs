using UnityEngine;

public class Projectile : MonoBehaviour {

    private float _speed = 1f;
    private Transform _target;

    private void Update() {
        if (_target != null) {
            Move();
        } else {
            MoveForward();
        }
    }

    public void SetTarget(Transform target) {
        this._target = target;
    }

    private Quaternion LookToTarget(Transform target) {
        Vector3 relativePos = target.position - transform.position;

        return Quaternion.LookRotation(relativePos);
    }

    private Quaternion LookToTarget(Vector3 direction) {
        return Quaternion.LookRotation(direction);
    }

    private void Move() {
        Vector3 direction = _target.position - transform.position;
        this.transform.SetPositionAndRotation(direction * _speed * Time.deltaTime, LookToTarget(_target));
    }

    private void MoveForward() {
        this.transform.SetPositionAndRotation(Vector3.forward * _speed * Time.deltaTime, LookToTarget(Vector3.forward));
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
    }

}
