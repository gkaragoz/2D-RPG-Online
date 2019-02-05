using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    public int AttackDamage { get { return _characterStats.GetAttackDamage(); } }
    public LivingEntity SelectedTarget { get { return _characterAttack.Target; } }
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

    public ParticleSystem takeHitFX;

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;
    private CharacterUI _characterUI;
    private CharacterStats _characterStats;
    private LivingEntity _livingEntity;

    private bool _isOfflineMode = false;

    private void Awake() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
        _characterUI = GetComponent<CharacterUI>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
        //_characterAttack.SearchTarget();

        //Attack();
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

    public void Attack() {
        _characterAttack.SearchTarget(_isOfflineMode);

        if (_characterAttack.CanAttack && !_characterStats.IsDeath()) {
            if (!_isOfflineMode && NetworkManager.mss != null) {
                _livingEntity.NetworkEntity.SendAttackInputData(SelectedTargetID);
            }

            _characterMotor.LookTo(_characterAttack.TargetPosition);
            _characterAttack.Attack();
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

}
