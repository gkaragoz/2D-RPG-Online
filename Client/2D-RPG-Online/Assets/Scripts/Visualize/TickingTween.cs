using System.Collections;
using UnityEngine;

public class TickingTween : MonoBehaviour {

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
    private float tickingAmount;

    private void Start() {
        StartCoroutine(Tick());
    }

    public IEnumerator Tick() {
        yield return new WaitForSeconds(delay);

        while (true) {
            LeanTween.value(_targetTransform.gameObject,
                new Vector3(0, 0, -tickingAmount),
                new Vector3(0, 0, tickingAmount),
                speed)
                .setEaseInOutCirc()
                .setOnComplete(onComplete)
                .setOnUpdate(
                (Vector3 val) => {
                    _targetTransform.rotation = Quaternion.Euler(val);
                }
            );

            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void onComplete() {
        _targetTransform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnValidate() {
        if (repeatRate <= 2 * speed) {
            repeatRate = (2 * speed + 0.1f);
        }
    }

}
