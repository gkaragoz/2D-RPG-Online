using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    public int AttackDamage { get { return _characterStats.GetAttackDamage(); } }
    public LivingEntity SelectedTarget { get { return _selectedTarget; } }
    public bool HasTarget {
        get {
            return _selectedTarget == null ? false : true;
        }
    }

    [Header("Initialization")]
    public LayerMask attackables;
    public ParticleSystem takeHitFX;

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;
    private CharacterUI _characterUI;
    private CharacterStats _characterStats;
    private LivingEntity _livingEntity;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private LivingEntity _selectedTarget;

    private bool _isOfflineMode = false;

    private void Awake() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
        _characterUI = GetComponent<CharacterUI>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
        //LivingEntity closestTarget = GetClosestTarget(_isOfflineMode);
        //if (closestTarget != null) {
        //    if (IsTargetInRange(closestTarget)) {
        //        if (!HasTarget) {
        //            AttackToTarget(closestTarget);
        //        } else {
        //            Attack();
        //        }
        //    }
        //}
    }

    public void Initialize(NetworkIdentifier networkObject, LivingEntity livingEntity, bool isOfflineMode) {
        this._isOfflineMode = isOfflineMode;
        this._livingEntity = livingEntity;
        if (!_isOfflineMode) {
            this._characterStats.Initialize(networkObject);
        }
        this._characterUI.UpdateUI();
        this._characterUI.UpdateTargetUI();
    }

    public void SelectTarget(LivingEntity livingEntity) {
        if (_selectedTarget != null) {
            DeselectTarget();
        }

        _selectedTarget = livingEntity;

        _characterUI.SelectTarget(livingEntity);
    }

    public void DeselectTarget() {
        _selectedTarget = null;

        _characterUI.DeselectTarget();
    }

    public LivingEntity GetClosestTarget(bool isOfflineMode) {
        LivingEntity target = null;
        float distance = Mathf.Infinity;

        if (isOfflineMode) {
            LivingEntity[] potantialTargets = GameObject.FindObjectsOfType<LivingEntity>();
            for (int ii = 0; ii < potantialTargets.Length; ii++) {
                LivingEntity potantialTarget = potantialTargets[ii];
                if (potantialTarget.EntityType == LivingEntity.Type.Player || potantialTarget.IsDeath) {
                    continue;
                }

                if (attackables == (attackables | (1 << potantialTarget.gameObject.layer))) {

                    float potantialTargetDistance = GetDistanceOf(potantialTarget.transform);

                    if (potantialTargetDistance < distance) {
                        target = potantialTarget;
                        distance = potantialTargetDistance;
                    }
                }
            }
        } else {
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
            _livingEntity.NetworkEntity.SendAttackInputData(GetTargetID(null));

            _characterAttack.AttackEmpty();
            _characterAnimator.OnAttack();
        }
    }

    public void AttackToTarget(LivingEntity target) {
        if (!_characterStats.IsDeath() && _characterAttack.CanAttack) {
            if (!HasTargetDied(target)) {
                if (IsTargetInRange(target)) {
                    _livingEntity.NetworkEntity.SendAttackInputData(GetTargetID(target));

                    _characterMotor.LookTo(target.transform.position);
                    _characterAttack.AttackToTarget(target);
                    _characterAnimator.OnAttack();
                    _characterUI.UpdateTargetUI();
                }
            }
        }
    }

    public void AttackAnimation(Vector3 direction) {
        if (!_characterStats.IsDeath()) {
            _characterMotor.LookTo(direction);
            _characterAnimator.OnAttack();
        }
    }

    public void MoveToInput(Vector3 input) {
        if (!_characterStats.IsDeath()) {
            if (!_isOfflineMode && NetworkManager.mss != null) {
                _livingEntity.NetworkEntity.SendMovementInputData(input);
            }

            _characterMotor.MoveToInput(input);
            _characterAnimator.OnMove(input);
        }
    }

    public void MoveToPosition(Vector3 position) {
        if (!_characterStats.IsDeath()) {
            _characterMotor.MoveToPosition(position);
            _characterAnimator.OnMove(position);
        }
    }

    public void TakeDamage(int damage) {
        if (AudioManager.instance != null) {
            AudioManager.instance.Play("OnHit");
        }

        _characterStats.TakeDamage(damage);
        takeHitFX.Play();

        if (_characterStats.GetCurrentHealth() <= 0) {
            Die();
            Debug.Log("Death");
        } else {
            _characterAnimator.OnHit();
            _characterUI.UpdateUI();
        }
    }

    public void Die() {
        _livingEntity.IsDeath = true;
        _characterAnimator.OnDeath();
        _characterUI.HideUI();
    }

    public void Stop() {
        _characterAnimator.OnStop();
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
