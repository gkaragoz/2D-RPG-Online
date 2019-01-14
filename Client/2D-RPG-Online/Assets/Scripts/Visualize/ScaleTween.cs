using System.Collections;
using UnityEngine;

public class ScaleTween : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private RectTransform _targetTransform;

    [Header("Settings")]
    [SerializeField]
    private float speed = 0.3f;
    [SerializeField]
    private float repeatRate;
    [SerializeField]
    private float delay;
    [SerializeField]
    private Vector3 scaleAmount;

    private void Start() {
        StartCoroutine(Scale());
    }

    public IEnumerator Scale() {
        yield return new WaitForSeconds(delay);

        while (true) {
            LeanTween.value(_targetTransform.gameObject,
                _targetTransform.localScale,
                _targetTransform.localScale + scaleAmount, speed)
                .setEaseInOutCirc()
                .setLoopPingPong()
                .setRepeat(2)
                .setOnUpdate(
                (Vector2 val) => {
                    _targetTransform.localScale = val;
                }
            );

            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void OnValidate() {
        if (repeatRate <= 2 * speed) {
            repeatRate = (2 * speed + 0.1f);
        }
    }

}
