using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage all inputs of players.
/// <list type="bullet">
/// Required Components:
/// <item>
/// <term>CharacterMotor</term>
/// <description>See <see cref="CharacterMotor"/></description>
/// </item>
/// <item>
/// <term>CharacterAttack</term>
/// <description>See <see cref="CharacterAttack"/></description>
/// </item>
/// <item>
/// <term>CharacterAnimator</term>
/// <description>See <see cref="CharacterAnimator"/></description>
/// </item>
/// </list>
/// </summary>
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
