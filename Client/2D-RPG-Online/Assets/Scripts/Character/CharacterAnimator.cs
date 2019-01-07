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
    /// Get component references.
    /// </summary>
    private void Start() {
        _animator = GetComponentInChildren< Animator>();
    }

    /// <summary>
    /// Handles character's run animations.
    /// </summary>
    /// <remarks>
    /// <para>If xValue on input is less than zero than this method flips the transform.localScale of this gameObject.</para>
    /// See <see cref="CharacterController.Move(Vector2)"/>
    /// </remarks>
    public void OnMove(Vector2 direction) {
        float xValue = direction.x;

        if (xValue < 0) {
            _animator.transform.localScale = new Vector3(-1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        } else {
            _animator.transform.localScale = new Vector3(1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        }

        _animator.SetBool("Run", true);
    }
    
    /// <summary>
    /// Stops the run animation.
    /// </summary>
    public void OnStop() {
        _animator.SetBool("Run", false);
    }
    
    /// <summary>
    /// Handles character's attack animations.
    /// See <see cref="CharacterController.Attack"/>
    /// </summary>
    public void OnAttack() {
        _animator.SetTrigger("Attack");
    }

}
