using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage and handle player's animations behaviours.
/// </summary>
public class PlayerAnimator : MonoBehaviour {

    /// <summary>
    /// Animator component.
    /// </summary>
    private Animator _animator;
    /// <summary>
    /// PlayerController class reference to be able to read user inputs.
    /// </summary>
    private PlayerController _playerController;
    /// <summary>
    /// PlayerAttack class reference to listen attack event.
    /// </summary>
    private PlayerAttack _playerAttack;

    /// <summary>
    /// Get component references.
    /// </summary>
    /// <remarks>
    /// <para>Add listener to onAttacking event.</para>
    /// See <see cref="OnAttack"/>
    /// </remarks>
    private void Start() {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _playerAttack = GetComponent<PlayerAttack>();

        _playerAttack.onAttacking += OnAttack;
    }

    /// <summary>
    /// Checking player's has input or not. If so, than handle the RUN animation of player.
    /// </summary>
    private void Update() {
        if (_playerController.HasInput) {
            float xValue = _playerController.CurrentDirection.x;

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
    /// Listening playerAttacks, if this function triggered, than Attack animation will be run.
    /// See <see cref="PlayerAttack.Attack"/>
    /// </summary>
    private void OnAttack() {
        _animator.SetTrigger("Attack");
    }

}
