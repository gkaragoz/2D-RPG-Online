using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour {

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

    private CharacterStats _characterStats;

    private void Start() {
        attackHitPoint.gameObject.SetActive(false);
        _characterStats = GetComponent<CharacterStats>();
    }

    public void Attack() {
        StartCoroutine(IAttack());
    }

    private IEnumerator IAttack() {
        if (AudioManager.instance != null) {
            AudioManager.instance.Play("swing" + Random.Range(1, 3));
        }

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        attackHitPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        attackHitPoint.gameObject.SetActive(false);

        yield return new WaitForSeconds(_characterStats.GetAttackSpeed());

        IsAttacking = false;
    }

    private void OnDrawGizmos() {
        if (attackHitPoint != null && _characterStats != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackHitPoint.position, _characterStats.GetAttackRange());
        }
    }

}
