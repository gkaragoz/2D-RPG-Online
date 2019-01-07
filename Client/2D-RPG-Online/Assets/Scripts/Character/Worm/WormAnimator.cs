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
            _wormTransform.transform.localScale = new Vector3(-1, _wormTransform.transform.localScale.y, _wormTransform.transform.localScale.z);
        } else {
            _wormTransform.transform.localScale = new Vector3(1, _wormTransform.transform.localScale.y, _wormTransform.transform.localScale.z);
        }

        _animator.SetTrigger("Jump");
    }

    public void OnAttack() {
        _animator.SetTrigger("Attack");
    }

    public void OnDeath() {
        _animator.SetTrigger("Death");
    }

}
