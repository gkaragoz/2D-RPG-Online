using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAttack : MonoBehaviour {

    public LayerMask attackables;

    public bool IsAttacking { get; private set; }

    public bool CanAttack {
        get {
            if (Time.time > _nextAttackTime) {
                if (!IsAttacking) {
                    if (!HasTargetDied) {
                        if (IsTargetInRange()) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public bool HasTarget {
        get {
            return Target == null ? false : true;
        }
    }

    public bool HasTargetDied {
        get {
            if (HasTarget) {
                return _target.CharacterController.IsDeath;
            }
            return false;
        }
    }

    public PlayerController Target { get { return _target; } }
    public Vector3 TargetPosition { get { return _target.transform.position; } }

    [SerializeField]
    [Utils.ReadOnly]
    private float _nextAttackTime;
    [SerializeField]
    [Utils.ReadOnly]
    private PlayerController _target;

    private CharacterStats _characterStats;

    private void Start() {
        _characterStats = GetComponent<CharacterStats>();
    }

    public void Attack() {
        StartCoroutine(IAttack());
    }

    public void SearchTarget() {
        PlayerController target = null;
        float distance = Mathf.Infinity;

        //for (int ii = 0; ii < RoomManager.instance.OtherPlayerControllers.Count; ii++) {
        List<PlayerController> potantialTargets = FindObjectsOfType<PlayerController>().ToList();
        potantialTargets.Remove(this.gameObject.GetComponent<PlayerController>());
        for (int ii = 0; ii < potantialTargets.Count; ii++) {

            PlayerController potantialTarget = potantialTargets[ii];
            if (attackables == (attackables | (1 << potantialTarget.gameObject.layer))) {

                float potantialTargetDistance = GetDistanceOf(potantialTarget.transform);

                if (potantialTargetDistance < distance) {
                    target = potantialTarget;
                }

            }
        }
        this._target = target;
    }

    private IEnumerator IAttack() {
        if (AudioManager.instance != null) {
            AudioManager.instance.Play("swing" + Random.Range(1, 3));
        }

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        yield return new WaitForSeconds(0.3f);

        _target.TakeDamage(_characterStats.GetAttackDamage());

        yield return new WaitForSeconds(_characterStats.GetAttackSpeed());

        IsAttacking = false;
    }

    private bool IsTargetInRange() {
        if (HasTarget) {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance <= _characterStats.GetAttackRange()) {
                return true;
            }
        }

        return false;
    }

    private float GetDistanceOf(Transform target) {
        return Vector3.Distance(transform.position, target.position);
    }

    private void OnDrawGizmos() {
        if (_characterStats != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _characterStats.GetAttackRange());
        }
    }

}
