using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

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

    public void Attack() {
        if (!_characterAttack.IsAttacking && _characterAttack.CanAttack) {
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void Move(Vector3 direction) {
        _characterMotor.Move(direction);
        _characterAnimator.OnMove(direction);
    }

    public void TakeDamage() {
        _characterAnimator.OnHit();
    }

    public void OnDeath() {
        _characterAnimator.OnDeath();
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
