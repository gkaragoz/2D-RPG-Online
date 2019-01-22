using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    private Animator _animator;

    private void Start() {
        _animator = GetComponentInChildren< Animator>();
    }

    public void OnMove(Vector3 direction) {
        _animator.SetFloat("InputMagnitude", direction.magnitude);
    }
    
    public void OnAttack() {
        _animator.SetTrigger("BasicAttack");
    }

    public void OnDeath() {
        _animator.SetTrigger("Die");
    }

}
