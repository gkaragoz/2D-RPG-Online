using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CanvasGroup))]
public class FadeInOut : MonoBehaviour {

    private Animator _animator;
    private CanvasGroup _canvasGroup;

    private const string FADE_IN_COMPLETE = "FadeInComplete";
    private const string FADE_OUT_COMPLETE = "FadeOutComplete";
    private const string FADE_IN = "FadeIn";
    private const string FADE_OUT = "FadeOut";
    private const string IS_LOCKED = "IsLocked";

    public bool IsLocked {
        get { return _animator.GetBool(IS_LOCKED); }
        set { SetBool(IS_LOCKED, value); }
    }

    public bool IsClosed {
        get { return _canvasGroup.alpha == 0 ? true : false; }
    }

    public bool IsOpened {
        get { return _canvasGroup.alpha == 1 ? true : false; }
    }

    private void Start() {
        _animator = GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_animator == null) {
            LogManager.instance.AddLog("[FadeInOut.cs] Animator not found!", Log.Type.Error);
        }
        if (_canvasGroup == null) {
            LogManager.instance.AddLog("[FadeInOut.cs] CanvasGroup not found!", Log.Type.Error);
        }
    }

    public void FadeIn() {
        if (IsClosed) {
            SetTrigger(FADE_IN);
        } else if (IsCurrentState(FADE_OUT)) {
            ShowImmediately(false);
        }
    }

    public void FadeOut() {
        if (IsOpened) {
            SetTrigger(FADE_OUT);
        }
    }

    public void ShowImmediately(bool lockedStatus) {
        IsLocked = lockedStatus;

        SetTrigger(FADE_IN_COMPLETE);
    }

    public void HideImmediately() {
        IsLocked = false;

        SetTrigger(FADE_OUT_COMPLETE);
    }

    private bool IsCurrentState(string name) {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private void SetTrigger(string name) {
        _animator.SetTrigger(name);
    }

    private void SetBool(string name, bool status) {
        _animator.SetBool(name, status);
    }

}
