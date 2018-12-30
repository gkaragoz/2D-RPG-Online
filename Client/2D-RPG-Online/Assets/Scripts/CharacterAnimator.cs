using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage and handle character's animations behaviours.
/// </summary>
public class CharacterAnimator : MonoBehaviour {

    /// <summary>
    /// Animator component.
    /// </summary>
    private Animator _animator;
    /// <summary>
    /// CharacterController class reference to be able to read user inputs.
    /// </summary>
    private CharacterController _characterController;
    /// <summary>
    /// CharacterAttack class reference to listen attack event.
    /// </summary>
    private CharacterAttack _playerAttack;

    /// <summary>
    /// Get component references.
    /// </summary>
    /// <remarks>
    /// <para>Add listener to onAttacking event.</para>
    /// See <see cref="OnAttack"/>
    /// </remarks>
    private void Start() {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _playerAttack = GetComponent<CharacterAttack>();

        _playerAttack.onAttacking += OnAttack;
    }

    /// <summary>
    /// Checking player's has input or not. If so, than handle the RUN animation of player.
    /// </summary>
    private void Update() {
        if (_characterController.IsMoving) {
            float xValue = _characterController.CurrentDirection.x;

            if (xValue < 0) {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            } else {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }

            _animator.SetBool("Run", true);
        } else {
            _animator.SetBool("Run", false);
        }
    }
    
    /// <summary>
    /// Listening characterAttacks, if this function triggered, than Attack animation will be run.
    /// See <see cref="CharacterAttack.Attack"/>
    /// </summary>
    private void OnAttack() {
        _animator.SetTrigger("Attack");
    }

}
