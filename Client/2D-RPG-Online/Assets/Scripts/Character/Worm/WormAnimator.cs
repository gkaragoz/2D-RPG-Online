using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAnimator : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Transform _wormTransform;

    private Animator _animator;

    private void Start() {
        _animator = GetComponent<Animator>();
    }

    public void OnJump(Vector2 direction) {
        float xValue = direction.x;

        if (xValue < 0) {
            _animator.transform.localScale = new Vector3(-1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        } else {
            _animator.transform.localScale = new Vector3(1, _animator.transform.localScale.y, _animator.transform.localScale.z);
        }

        _animator.transform.position = _wormTransform.position + new Vector3(direction.x, direction.y, 0);
        _animator.SetTrigger("Jump");
    }

    public void OnAttack() {
        _animator.SetTrigger("Attack");
    }

    public void OnDeath() {
        _animator.SetTrigger("Death");
    }

}
