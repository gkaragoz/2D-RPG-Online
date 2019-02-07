using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public float locomotionAnimationSmoothTime = 0.1f;
    public bool IsSitting { get { return _animator.GetBool(anim_IsSitting); } }

    [SerializeField]
    private PathFollower _pathFollower;

    private Animator _animator;

    private static readonly int anim_OnHit = Animator.StringToHash("OnHit");
    private static readonly int anim_BasicAttack = Animator.StringToHash("BasicAttack");
    private static readonly int anim_InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int anim_Die = Animator.StringToHash("Die");
    private static readonly string anim_IsSitting = "IsSitting";

    private void Update() {
        if (_pathFollower != null) {
            if (_pathFollower.IsRunning) {
                OnMove(_pathFollower.DesiredPointPosition);
            }
        }
    }

    public void SetAnimator(Animator animator) {
        this._animator = animator;
    }

    public void OnMove(Vector3 direction) {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetFloat(anim_InputMagnitude, direction.magnitude, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    public void OnStop() {
        _animator.SetFloat(anim_InputMagnitude, 0, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    public void OnHit() {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetTrigger(anim_OnHit);
    }

    public void OnAttack() {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetTrigger(anim_BasicAttack);
    }

    public void OnMoveExample() {
        _animator.SetFloat(anim_InputMagnitude, 1f);
    }

    public void OnStopExample() {
        _animator.SetFloat(anim_InputMagnitude, 0f);
    }

    public void Sit() {
        _animator.SetBool(anim_IsSitting, true);
    }

    public void StandUp() {
        _animator.SetBool(anim_IsSitting, false);
    }

    public void OnDeath() {
        if (IsSitting) {
            StandUp();
        }
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
        _animator.SetBool(anim_IsSitting, false);
        transform.position = Vector3.zero;
    }

}
