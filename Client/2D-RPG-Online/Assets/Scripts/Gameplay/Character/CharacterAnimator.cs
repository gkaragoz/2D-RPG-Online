using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public float locomotionAnimationSmoothTime = 0.1f;
    public bool IsSitting { get { return _animator.GetBool(anim_IsSitting); } }

    [SerializeField]
    private PathFollower _pathFollower;

    [SerializeField]
    private Animator _animator;

    private static readonly int anim_OnHit = Animator.StringToHash("OnHit");
    private static readonly int anim_BasicAttack = Animator.StringToHash("BasicAttack");
    private static readonly int anim_InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int anim_Die = Animator.StringToHash("Die");
    private static readonly string anim_IsSitting = "IsSitting";

    private void Awake() {
        CharacterController characterController = GetComponent<CharacterController>();

        characterController.onAttack += OnAttack;
        characterController.onDeath += OnDeath;
        characterController.onTakeDamage += OnTakeDamage;
        characterController.onMove += OnMove;
        characterController.onStop += OnStop;
    }

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

    public void Reset() {
        _animator.ResetTrigger(anim_BasicAttack);
        _animator.ResetTrigger(anim_OnHit);
        _animator.ResetTrigger(anim_Die);

        _animator.SetFloat(anim_InputMagnitude, 0f);
        _animator.SetBool(anim_IsSitting, false);
        transform.position = Vector3.zero;
    }

    public void Sit() {
        _animator.SetBool(anim_IsSitting, true);
    }

    public void StandUp() {
        _animator.SetBool(anim_IsSitting, false);
    }

    private void OnMove(Vector3 direction) {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetFloat(anim_InputMagnitude, direction.magnitude, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    private void OnStop() {
        _animator.SetFloat(anim_InputMagnitude, 0, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    private void OnTakeDamage(int damage) {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetTrigger(anim_OnHit);
    }

    private void OnAttack(int targetID) {
        if (IsSitting) {
            StandUp();
        }
        _animator.SetTrigger(anim_BasicAttack);
    }

    private void OnDeath() {
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

}
