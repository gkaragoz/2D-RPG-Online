using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    public int AttackDamage { get { return _characterStats.GetAttackDamage(); } }

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

    public void InitializeOffline(LivingEntity livingEntity) {
        this._livingEntity = livingEntity;
        this._isOfflineMode = true;
    }

    public void Initialize(NetworkIdentifier networkObject, LivingEntity livingEntity) {
        this._characterStats.Initialize(networkObject);
        this._livingEntity = livingEntity;
        this._characterAnimator.Initialize(_livingEntity.onDeathEvent);
    }

    public void Attack() {
        _characterAttack.SearchTarget(_isOfflineMode);

        if (_characterAttack.CanAttack && !_livingEntity.IsDeath) {
            _characterMotor.LookTo(_characterAttack.TargetPosition);
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void MoveToInput(Vector3 input) {
        if (!_livingEntity.IsDeath) {
            _characterMotor.MoveToInput(input);
            _characterAnimator.OnMove(input);
        }
    }

    public void MoveToPosition(Vector3 position) {
        if (!_livingEntity.IsDeath) {
            _characterMotor.MoveToPosition(position);
            _characterAnimator.OnMove(position);
        }
    }

    public void TakeDamage(int damage) {
        _characterStats.TakeDamage(damage);
        takeHitFX.Play();

        if (_characterStats.GetCurrentHealth() <= 0) {
            OnDeath();
            Debug.Log("Death");
        } else {
            _characterAnimator.OnHit();
            _characterUI.UpdateUI();
        }
    }

    public void OnDeath() {
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
