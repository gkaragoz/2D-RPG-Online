using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WormMotor))]
public class WormAI : MonoBehaviour {

    private WormMotor _wormMotor;
    private WormAnimator _wormAnimator;

    private void Start() {
        _wormMotor = GetComponent<WormMotor>();
        _wormAnimator = GetComponent<WormAnimator>();

        StartCoroutine(Jump());
    }

    private IEnumerator Jump() {
        while (true) {
            yield return new WaitForSeconds(1f);

            _wormMotor.Jump(Vector2.right);
            _wormAnimator.OnJump(Vector2.right);
        }
    }
	
}
