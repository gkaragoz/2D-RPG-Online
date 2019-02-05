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

    public void EmptyAttack() {
        StartCoroutine(IEmptyAttack());
    }

    public void AttackToTarget(LivingEntity target) {
        StartCoroutine(IAttackToTarget(target));
    }

    private IEnumerator IEmptyAttack() {
        Debug.Log("Empty Attack");

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("swing" + Random.Range(1, 3));
        }

        _nextAttackTime = Time.time + _characterStats.GetAttackSpeed();

        IsAttacking = true;

        yield return new WaitForSeconds(0.3f);

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

        yield return new WaitForSeconds(0.3f);

        if (target.EntityType == LivingEntity.Type.Player) {
            target.GetComponent<PlayerController>().TakeDamage(_characterStats.GetAttackDamage());
        } else if (target.EntityType == LivingEntity.Type.BOT) {
            target.GetComponent<BotController>().TakeDamage(_characterStats.GetAttackDamage());
        }

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
