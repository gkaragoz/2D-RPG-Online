using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    private Animator _animator;

    private void Start() {
        _animator = GetComponentInChildren< Animator>();
    }

    public void OnMove(Vector2 direction) {
        float xValue = direction.x;

        if (xValue < 0) {
            _animator.transform.localScale = new Vector3(-1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        } else {
            _animator.transform.localScale = new Vector3(1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        }

        if (direction != Vector2.zero) {
            _animator.SetBool("IsRunning", true);
        } else {
            _animator.SetBool("IsRunning", false);
        }
    }
    
    public void OnStop() {
        _animator.SetBool("IsRunning", false);
    }
    
    public void OnAttack() {
        _animator.SetTrigger("Attack");
    }

}
