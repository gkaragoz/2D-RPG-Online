using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public delegate void AttackEvent();
    public event AttackEvent onAttacking;

    [Header("Initialization")]
    public float attackSpeed = 1f;
    public float attackRangeX = 0.35f;
    public float attackRangeY = 0.35f;
    public LayerMask attackables;
    public Transform attackHitPoint;

    public bool IsAttacking { get; private set; }
    public bool CanAttack {
        get {
            return Time.time > _nextAttackTime;
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private float _nextAttackTime;

    private void Start() {
        _nextAttackTime = 2f;
    }

    public void Attack() {
        _nextAttackTime = Time.time + attackSpeed;

        Collider2D[] objectsToDamage = Physics2D.OverlapBoxAll(attackHitPoint.position, new Vector2(attackRangeX, attackRangeY), 0f, attackables);
        for (int ii = 0; ii < objectsToDamage.Length; ii++) {
            //Run on damage function in the victim side.
            objectsToDamage[ii].GetComponent<Destructable>().OnDamageTaken();
        }

        IsAttacking = true;

        if (onAttacking != null) {
            onAttacking.Invoke();
        }
    }

    public void OnHit() {
        Debug.Log("On Hit.");
        IsAttacking = false;
    }

    private void OnDrawGizmos() {
        if (attackHitPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackHitPoint.position, new Vector3(attackRangeX, attackRangeY, 1f));
        }
    }

}
