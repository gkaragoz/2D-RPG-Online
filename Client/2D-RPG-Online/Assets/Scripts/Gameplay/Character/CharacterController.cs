using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    public bool IsDeath { get { return _isDeath; } }

    [SerializeField]
    [Utils.ReadOnly]
    private bool _isDeath;

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;
    private CharacterUI _characterUI;
    private CharacterStats _characterStats;

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

    public void Attack() {
        if (_characterAttack.CanAttack && !_isDeath) {
            _characterMotor.LookTo(_characterAttack.TargetPosition);
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void MoveToInput(Vector3 input) {
        if (!_isDeath) {
            _characterMotor.MoveToInput(input);
            _characterAnimator.OnMove(input);
        }
    }

    public void MoveToPosition(Vector3 position) {
        if (!_isDeath) {
            _characterMotor.MoveToPosition(position);
            _characterAnimator.OnMove(position);
        }
    }

    public void TakeDamage() {
        _characterAnimator.OnHit();
        _characterUI.UpdateUI();
    }

    public void OnDeath() {
        _isDeath = true;
        _characterAnimator.OnDeath();
        _characterUI.UpdateUI();
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
