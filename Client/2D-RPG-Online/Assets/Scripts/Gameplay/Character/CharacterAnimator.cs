using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    private Animator _animator;

    private void Start() {
        _animator = GetComponentInChildren< Animator>();
    }

    public void OnMove(Vector3 direction) {
        if (direction != Vector3.zero) {
            _animator.SetBool("IsRunning", true);
        } else {
            _animator.SetBool("IsRunning", false);
        }
    }
    
    public void OnStop() {
        _animator.SetBool("IsRunning", false);
    }
    
    public void OnAttack() {
        _animator.SetTrigger("BasicAttack");
    }

    public void OnDeath() {
        _animator.SetTrigger("Die");
    }

}
