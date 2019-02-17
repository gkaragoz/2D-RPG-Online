using System;
using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    public Action onInitialized;
    public Action<Vector3> onMove;
    public Action onStop;
    public Action<int> onAttack;
    public Action onDeath;
    public Action<int> onTakeDamage;
    public Action<int> onHealthRegenerated;

    public int AttackDamage { get { return _characterStats.GetAttackDamage(); } }
    public LivingEntity SelectedTarget { get { return _selectedTarget; } }
    public bool HasTarget {
        get {
            return _selectedTarget == null ? false : true;
        }
    }
    public CharacterStats CharacterStats { get { return _characterStats; } }

    [Header("Initialization")]
    public LayerMask attackables;

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;
    private CharacterUI _characterUI;
    private CharacterStats _characterStats;
    private SkillController _skillController;
    private LivingEntity _livingEntity;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private LivingEntity _selectedTarget;

    private void Awake() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
        _characterUI = GetComponent<CharacterUI>();
        _characterStats = GetComponent<CharacterStats>();
        _skillController = GetComponent<SkillController>();
    }

    public void Initialize(NetworkIdentifier networkObject, LivingEntity livingEntity) {
        this._livingEntity = livingEntity;
        this._characterStats.Initialize(networkObject);
        this._characterAnimator.SetAnimator(livingEntity.CharacterObject.GetComponent<Animator>());

        onInitialized?.Invoke();
    }

    public void SelectTarget(LivingEntity livingEntity) {
        if (_selectedTarget != null) {
            DeselectTarget();
        }

        _selectedTarget = livingEntity;
    }

    public void DeselectTarget() {
        _selectedTarget = null;
    }

    public void AutoAttack() {
        if (!HasTarget) {
            LivingEntity closestTarget = GetClosestTarget();
            if (closestTarget != null) {
                if (IsTargetInRange(closestTarget)) {
                    AttackToTarget(closestTarget);
                }
            }
        }
    }

    public LivingEntity GetClosestTarget() {
        LivingEntity target = null;
        float distance = Mathf.Infinity;

        for (int ii = 0; ii < RoomManager.instance.OtherPlayersCount; ii++) {
            LivingEntity potantialTarget = RoomManager.instance.GetPlayerByIndex(ii);

            if (potantialTarget.IsDeath) {
                continue;
            }

            if (attackables == (attackables | (1 << potantialTarget.gameObject.layer))) {

                float potantialTargetDistance = GetDistanceOf(potantialTarget.transform);

                if (potantialTargetDistance < distance) {
                    target = potantialTarget;
                }
            }
        }

        return target;
    }

    public void Attack() {
        if (_selectedTarget != null) {
            AttackToTarget(_selectedTarget);
        } else { 
            AttackEmpty();
        }
    }

    public void AttackEmpty() {
        if (!_characterStats.IsDeath() && _characterAttack.CanAttack) {
            _characterAttack.AttackEmpty();

            _skillController.OnAttack(SkillDatabase.instance.GetBasicAttackSkill(_livingEntity.PlayerClass));

            onAttack?.Invoke(GetTargetID(null));
        }
    }

    public void AttackToTarget(LivingEntity target) {
        if (!_characterStats.IsDeath() && _characterAttack.CanAttack) {
            if (!HasTargetDied(target)) {
                if (IsTargetInRange(target)) {
                    _characterMotor.LookTo(target.transform.position);
                    _characterAttack.AttackToTarget(target);

                    _skillController.OnAttack(SkillDatabase.instance.GetBasicAttackSkill(_livingEntity.PlayerClass), target.transform);

                    onAttack?.Invoke(GetTargetID(target));
                }
            }
        }
    }

    public void MoveToInput(Vector3 input) {
        if (!_characterStats.IsDeath()) {
            onMove?.Invoke(input);

            _characterMotor.MoveToInput(input);
        }
    }

    public void MoveToPosition(Vector3 position) {
        if (!_characterStats.IsDeath()) {
            _characterMotor.MoveToPosition(position);

            onMove?.Invoke(position);
        }
    }

    public void RegenerateHealth(int healthAmount) {
        _characterStats.ApplyHealth(healthAmount);

        onHealthRegenerated?.Invoke(healthAmount);
    }

    public void TakeDamage(Skill_SO.Skill_Name skillName, int damage) {
        _characterStats.TakeDamage(damage);

        _skillController.OnTakeDamage(SkillDatabase.instance.GetSkill(skillName));

        if (_characterStats.GetCurrentHealth() <= 0) {
            Die();
        } else {
            onTakeDamage?.Invoke(damage);
        }
    }

    public void Die() {
        _livingEntity.IsDeath = true;
        onDeath?.Invoke();
    }

    public void Stop() {
        onStop?.Invoke();
    }

    public void Rotate(Vector3 direction) {
        _characterMotor.Rotate(direction);
    }

    private int GetTargetID (LivingEntity target) { 
        int targetID = -1;
        if (target != null) {
            if (target.NetworkEntity != null) {
                targetID = target.NetworkEntity.Oid;
            }
        }

        return targetID;
    }

    private bool HasTargetDied(LivingEntity target) {
        return target.IsDeath;
    }

    private bool IsTargetInRange(LivingEntity target) {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= _characterStats.GetAttackRange()) {
            return true;
        }

        return false;
    }

    private float GetDistanceOf(Transform target) {
        return Vector3.Distance(transform.position, target.position);
    }

}
