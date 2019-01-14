using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack), typeof(CharacterAnimator))]
public class CharacterController : MonoBehaviour {

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;
    private CharacterAnimator _characterAnimator;

    private void Start() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
        _characterAnimator = GetComponent<CharacterAnimator>();
    }

    public void Attack() {
        if (!_characterAttack.IsAttacking && _characterAttack.CanAttack) {
            _characterAttack.Attack();
            _characterAnimator.OnAttack();
        }
    }

    public void Move(Vector2 direction) {
        if (!_characterAttack.IsAttacking) {
            _characterMotor.Move(direction);
            _characterAnimator.OnMove(direction);
        }
    }

    public void Stop() {
        _characterAnimator.OnStop();
    }

}
