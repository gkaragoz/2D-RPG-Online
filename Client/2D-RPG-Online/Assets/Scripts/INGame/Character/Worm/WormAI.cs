using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WormMotor))]
public class WormAI : MonoBehaviour {

    [SerializeField]
    [Range(0.1f, 1f)]
    private float _randomSpeed;

    private WormMotor _wormMotor;
    private WormAnimator _wormAnimator;

    private void Start() {
        _wormMotor = GetComponent<WormMotor>();
        _wormAnimator = GetComponent<WormAnimator>();

        StartCoroutine(Jump());
    }

    private IEnumerator Jump() {
        while (true) {
            yield return new WaitForSeconds(_wormMotor.speed + Random.Range(0f, _randomSpeed));

            Vector2 position = GetRandomPosition();

            _wormMotor.JumpToPosition(position);
            _wormAnimator.OnJump(position);
        }
    }

    private Vector2 GetRandomPosition() {
        float posX = Random.Range(-1f, 1f);
        float posY = Random.Range(-1f, 1f);

        return new Vector2(posX, posY).normalized;
    }
	
}
