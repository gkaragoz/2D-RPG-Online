﻿using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour {

    public bool IsAttacking { get; private set; }

    public bool CanAttack {
        get {
            if (Time.time > _nextAttackTime) {
                if (!IsAttacking) {
                    return true;
                }
            }
            return false;
        }
    }

    [SerializeField]
    [Utils.ReadOnly]
    private float _nextAttackTime;

    private CharacterStats _characterStats;

    private void Start() {
        _characterStats = GetComponent<CharacterStats>();
    }

    public void AttackEmpty() {
        StartCoroutine(IAttackEmpty());
    }

    public void AttackToTarget(LivingEntity target) {
        StartCoroutine(IAttackToTarget(target));
    }

    private IEnumerator IAttackEmpty() {
        Debug.Log("Attack Empty");

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        yield return new WaitForSeconds(_characterStats.GetAttackSpeed());

        IsAttacking = false;
    }

    private IEnumerator IAttackToTarget(LivingEntity target) {
        Debug.Log("Attack to target: " + target.name);

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        yield return new WaitForSeconds(_characterStats.GetAttackSpeed());

        IsAttacking = false;
    }

    private void OnDrawGizmos() {
        if (_characterStats != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _characterStats.GetAttackRange());
        }
    }

}
