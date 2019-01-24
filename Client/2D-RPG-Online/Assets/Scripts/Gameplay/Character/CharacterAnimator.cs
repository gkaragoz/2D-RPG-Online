using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public float locomotionAnimationSmoothTime = 0.1f;

    private static readonly int anim_OnHit = Animator.StringToHash("OnHit");
    private static readonly int anim_BasicAttack = Animator.StringToHash("BasicAttack");
    private static readonly int anim_InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int anim_Die = Animator.StringToHash("Die");

    private Animator _animator;

    private void Start() {
        _animator = GetComponentInChildren<Animator>();
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

    public void OnDeath() {
        _animator.SetLayerWeight(0, 0);
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 0);
        _animator.SetTrigger(anim_Die);
    }

}
