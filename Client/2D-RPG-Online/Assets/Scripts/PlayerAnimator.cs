﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private Animator _animator;
    private PlayerController _playerController;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _playerController.onAttacking += OnAttack;
    }

    private void Update() {
        if (_playerController.HasInput) {
            float xValue = _playerController.CurrentDirection.x;
            float yValue = _playerController.CurrentDirection.y;

            if (xValue < 0) {
                _spriteRenderer.flipX = true;
            } else {
                _spriteRenderer.flipX = false;
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
