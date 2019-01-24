using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public float locomotionAnimationSmoothTime = 0.1f;

    private Animator _animator;

    private void Start() {
        _animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(Vector3 direction) {
        _animator.SetFloat("InputMagnitude", direction.magnitude, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    public void OnStop() {
        _animator.SetFloat("InputMagnitude", 0, locomotionAnimationSmoothTime, Time.deltaTime);
    }
    
    public void OnAttack() {
        _animator.SetTrigger("BasicAttack");
    }

    public void OnDeath() {
        _animator.SetTrigger("Die");
    }

}
