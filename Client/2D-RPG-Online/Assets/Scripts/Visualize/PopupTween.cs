using System.Collections;
using UnityEngine;

public class PopupTween : MonoBehaviour {

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
    private Vector2 scaleAmount;

    private void Start() {
        StartCoroutine(Popup());
    }

    public IEnumerator Popup() {
        yield return new WaitForSeconds(delay);

        while (true) {
            LeanTween.value(_targetTransform.gameObject,
                _targetTransform.sizeDelta,
                _targetTransform.sizeDelta + scaleAmount, speed)
                .setEaseInOutCirc()
                .setLoopPingPong()
                .setRepeat(2)
                .setOnUpdate(
                (Vector2 val) => {
                    _targetTransform.sizeDelta = val;
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
