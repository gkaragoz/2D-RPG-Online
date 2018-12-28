using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

    [Header("Initialization")]
    public int health = 3;

    private Animator _animator;

    private void Start() {
        _animator = GetComponent<Animator>();
    }

    private void Destruct() {
        _animator.SetTrigger("Destruct");
    }

    public void ShakeCamera() {
        CameraShaker.Instance.ShakeOnce(Random.Range(1f, 2f), 5f, 0.1f, 1f);
    }

    public void OnDamageTaken() {
        if (health <= 0) {
            return;
        }

        _animator.SetTrigger("Damage");
        health--;

        if (health <= 0) {
            Destruct();
        }
    }

}
