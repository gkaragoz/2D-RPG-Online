using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage character's attacks.
/// </summary>
public class CharacterAttack : MonoBehaviour {

    /// <summary>
    /// Attack Event registers.
    /// </summary>
    public delegate void AttackEvent();

    /// <summary>
    /// This event called on character goes to attack.
    /// </summary>
    /// See <see cref="Attack"/>
    public event AttackEvent onAttacking;

    /// <summary>
    /// Character's attack speed. Lower value is quicker attack.
    /// </summary>
    [Header("Initialization")]
    public float attackSpeed = 1f;

    /// <summary>
    /// Character's attackRange on X size.
    /// </summary>
    public float attackRangeX = 0.35f;

    /// <summary>
    /// Character's attackRange on Y size.
    /// </summary>
    public float attackRangeY = 0.35f;

    /// <summary>
    /// A layerMask for detect that particular character can attack to it.
    /// </summary>
    public LayerMask attackables;

    /// <summary>
    /// A Transform component for attackRange's center.
    /// </summary>
    public Transform attackHitPoint;

    /// <summary>
    /// Is character attacking or not.
    /// </summary>
    public bool IsAttacking { get; private set; }

    /// <summary>
    /// Is character be able to attack or not depends on attack speed.
    /// </summary>
    public bool CanAttack {
        get {
            return Time.time > _nextAttackTime;
        }
    }

    /// <summary>
    /// Defines next attack time.
    /// </summary>
    [SerializeField]
    [Utils.ReadOnly]
    private float _nextAttackTime;

    /// <summary>
    /// This function does the real job.
    /// </summary>
    /// <remarks>
    /// <para>Plays swing sound effect.</para>
    /// <para>Checks overlappedBox around the attackHitPoint than attack them</para>
    /// </remarks>
    public void Attack() {
        AudioManager.instance.Play("swing" + Random.Range(1, 3));

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

    /// <summary>
    /// This function is triggered on Attack animation key event.
    /// </summary>
    public void OnHit() {
        Debug.Log("On Hit.");
        IsAttacking = false;
    }

    /// <summary>
    /// This function shows attack range of character's hitPoint.
    /// </summary>
    private void OnDrawGizmos() {
        if (attackHitPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackHitPoint.position, new Vector3(attackRangeX, attackRangeY, 1f));
        }
    }

}
