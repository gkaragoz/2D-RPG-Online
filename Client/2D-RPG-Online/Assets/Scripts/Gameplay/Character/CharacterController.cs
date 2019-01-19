using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;

    private void Awake() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Update() {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) {
            _characterAnimator.OnDeath();
        }
    }

    public void Initiailize(PlayerObject playerData) {
        _characterMotor.SetMovementSpeed(playerData.MovementSpeed);
    }

    public void Attack() {
        if (!_characterAttack.IsAttacking && _characterAttack.CanAttack) {
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void Move(Vector3 direction) {
        if (!_characterAttack.IsAttacking) {
            _characterMotor.Move(direction);
            _characterAnimator.OnMove(direction);
        }
    }

    public void ToNewPosition(Vector3 newPosition) {
        if (new Vector3(newPosition.x, newPosition.z) == transform.position) {
            _characterAnimator.OnStop();
        } else {
            Vector3 direction = new Vector3(newPosition.x, newPosition.z) - transform.position;

            _characterAnimator.OnMove(direction);

            transform.position = newPosition;
        }
    }

    public void Stop() {
        _characterAnimator.OnStop();
    }

}
