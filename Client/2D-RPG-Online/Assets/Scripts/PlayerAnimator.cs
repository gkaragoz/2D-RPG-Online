using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private Animator _animator;
    private PlayerController _playerController;
    private PlayerAttack _playerAttack;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _playerAttack = GetComponent<PlayerAttack>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _playerAttack.onAttacking += OnAttack;
    }

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

    private void OnAttack() {
        _animator.SetTrigger("Attack");
    }

}
