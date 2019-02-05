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
    public int SelectedTargetID {
        get {
            int targetID = -1;
            if (SelectedTarget != null) {
                targetID = SelectedTarget.NetworkEntity.Oid;
            }
            _livingEntity.NetworkEntity.SendAttackInputData(targetID);

            return targetID;
        }
    }

    public bool HasTargetDied {
        get {
            if (HasTarget) {
                return _selectedTarget.IsDeath;
            }
            return false;
        }
    }

    public bool IsTargetInRange() {
        if (HasTarget) {
            float distance = Vector3.Distance(transform.position, _selectedTarget.transform.position);
            if (distance <= _characterStats.GetAttackRange()) {
                return true;
            }
        }

        return false;
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
        //if (!HasTarget) {
        //    SearchTarget(_isOfflineMode);
        //}
    }

    public void Initialize(NetworkIdentifier networkObject, LivingEntity livingEntity, bool isOfflineMode) {
        this._isOfflineMode = isOfflineMode;
        this._livingEntity = livingEntity;
        if (!_isOfflineMode) {
            this._characterStats.Initialize(networkObject);
        }
        this._characterAnimator.Initialize(_characterStats.onDeathEvent);
        this._characterUI.UpdateUI();
    }

    public void SelectTarget(LivingEntity livingEntity) {
        _selectedTarget = livingEntity;
    }

    public void DeselectTarget() {
        _selectedTarget = null;
    }

    public void SearchTarget(bool isOfflineMode) {
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
                if (attackables == (attackables | (1 << potantialTarget.gameObject.layer))) {

                    float potantialTargetDistance = GetDistanceOf(potantialTarget.transform);

                    if (potantialTargetDistance < distance) {
                        target = potantialTarget;
                    }
                }
            }
        }

        SelectTarget(target);
    }

    public void Attack() {
        if (_characterAttack.CanAttack && !_characterStats.IsDeath()) {
            if (!_isOfflineMode && NetworkManager.mss != null) {
                _livingEntity.NetworkEntity.SendAttackInputData(SelectedTargetID);
            }

            if (!HasTargetDied && IsTargetInRange()) {
                _characterMotor.LookTo(_selectedTarget.transform.position);
                _characterAttack.AttackToTarget(_selectedTarget);
            } else {
                _characterAttack.EmptyAttack();
            }

            _characterAnimator.OnAttack();
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

    public void UpdateUI() {
        _characterUI.UpdateUI();
    }

    private float GetDistanceOf(Transform target) {
        return Vector3.Distance(transform.position, target.position);
    }

}
