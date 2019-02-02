using System;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public float locomotionAnimationSmoothTime = 0.1f;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private PathFollower _pathFollower;

    private static readonly int anim_OnHit = Animator.StringToHash("OnHit");
    private static readonly int anim_BasicAttack = Animator.StringToHash("BasicAttack");
    private static readonly int anim_InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int anim_Die = Animator.StringToHash("Die");

    private void Update() {
        if (_pathFollower != null) {
            if (_pathFollower.IsRunning) {
                OnMove(_pathFollower.DesiredPointPosition);
            }
        }
    }

    public void Initialize(Action onDeathEvent) {
        onDeathEvent += OnDeath;
    }

    public void OnMove(Vector3 direction) {
        _animator.SetFloat(anim_InputMagnitude, direction.magnitude, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    public void OnStop() {
        _animator.SetFloat(anim_InputMagnitude, 0, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    public void OnHit() {
        _animator.SetTrigger(anim_OnHit);
    }

    public void OnAttack() {
        _animator.SetTrigger(anim_BasicAttack);
    }

    public void OnMoveExample() {
        _animator.SetFloat(anim_InputMagnitude, 1f);
    }

    public void OnStopExample() {
        _animator.SetFloat(anim_InputMagnitude, 0f);
    }

    public void OnDeath() {
        if (_pathFollower != null) {
            if (_pathFollower.IsRunning) {
                _pathFollower.Stop();
            }
        }

        _animator.SetLayerWeight(0, 0);
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);
        _animator.SetTrigger(anim_Die);
    }

    public void Reset() {
        _animator.ResetTrigger(anim_BasicAttack);
        _animator.ResetTrigger(anim_OnHit);
        _animator.ResetTrigger(anim_Die);

        _animator.SetFloat(anim_InputMagnitude, 0f);
        transform.position = Vector3.zero;
    }

}
