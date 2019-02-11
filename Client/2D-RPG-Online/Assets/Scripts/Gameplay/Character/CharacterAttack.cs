using System.Collections;
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

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("swing" + Random.Range(1, 3));
        }

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        yield return new WaitForSeconds(_characterStats.GetAttackSpeed());

        IsAttacking = false;
    }

    private IEnumerator IAttackToTarget(LivingEntity target) {
        Debug.Log("Attack to target: " + target.name);

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("swing" + Random.Range(1, 3));
        }

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        if (target.EntityType == LivingEntity.Type.Player) {
            target.GetComponent<PlayerController>().TakeDamage(_characterStats.GetAttackDamage());
        } else if (target.EntityType == LivingEntity.Type.BOT) {
        } else if (target.EntityType == LivingEntity.Type.Object) { }

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
