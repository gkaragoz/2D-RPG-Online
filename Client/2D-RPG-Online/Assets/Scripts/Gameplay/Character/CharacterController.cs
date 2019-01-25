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

    private void Start() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
        _characterUI = GetComponent<CharacterUI>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
        _characterAttack.SearchTarget();

        Attack();
    }

    public void Attack() {
        if (_characterAttack.CanAttack && !_isDeath) {
            _characterMotor.LookTo(_characterAttack.TargetPosition);
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void Move(Vector3 direction) {
        if (!_isDeath) {
            _characterMotor.Move(direction);
            _characterAnimator.OnMove(direction);
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

    public void ToNewPosition(Vector3 newPosition) {
        if (new Vector3(newPosition.x, newPosition.z) != transform.position) {
            Vector3 direction = new Vector3(newPosition.x, 0, newPosition.z) - transform.position;

            _characterAnimator.OnMove(direction);

            Vector3 rotation = new Vector3(direction.x, 0f, direction.z);

            if (rotation != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(rotation);
            }

            transform.position = newPosition;
        }
    }

    public void UpdateUI() {
        _characterUI.UpdateUI();
    }

    public void ShowControllers() {
        _characterUI.Show();
    }

    public void HideControllers() {
        _characterUI.Hide();
    }

}
