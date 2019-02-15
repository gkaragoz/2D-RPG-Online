using TMPro;
using UnityEngine;

public class DamageIndicator : Menu, IPooledObject {

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private TextMeshProUGUI _txtDamage;
    [SerializeField]
    private Color _damageColor;
    [SerializeField]
    private Color _healColor;
    [SerializeField]
    [Utils.ReadOnly]
    private float _clipLength;

    private void Start() {
        AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

        _clipLength = clipInfo[0].clip.length;
    }

    public void OnObjectReused() {
    }

    public void Play() {
        Invoke("SetDeactive", _clipLength);
    }

    public void SetText(string text) {
        _txtDamage.text = text;
    }

    public void SetColor(DamageIndicatorController.Type type) {
        switch (type) {
            case DamageIndicatorController.Type.Damage:
                _txtDamage.color = _damageColor;
                break;
            case DamageIndicatorController.Type.Heal:
                _txtDamage.color = _healColor;
                break;
            default:
                break;
        }
    }

    private void SetDeactive() {
        gameObject.SetActive(false);
    }
}
