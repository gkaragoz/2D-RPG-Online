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
/// </list>
/// </summary>
[RequireComponent(typeof(CharacterMotor), typeof(CharacterAttack))]
public class CharacterController : MonoBehaviour {

    [SerializeField]
    [Utils.ReadOnly]
    private Vector2 _currentDirection;

    public Vector2 CurrentDirection {
        get {
            return _currentDirection;
        }
        private set {
            _currentDirection = value;
        }
    }

    public bool IsMoving {
        get {
            return _characterMotor.IsMoving;
        }
    }

    private CharacterMotor _characterMotor;
    private CharacterAttack _characterAttack;

    private void Start() {
        _characterMotor = GetComponent<CharacterMotor>();
        _characterAttack = GetComponent<CharacterAttack>();
    }

    public void Attack() {
        if (!_characterAttack.IsAttacking && _characterAttack.CanAttack) {
            _characterAttack.Attack();
        }
    }

    public void Move(Vector2 direction) {
        CurrentDirection = direction;

        if (!_characterAttack.IsAttacking) {
            _characterMotor.Move(CurrentDirection);
        }

        CurrentDirection = Vector2.zero;
    }

}
