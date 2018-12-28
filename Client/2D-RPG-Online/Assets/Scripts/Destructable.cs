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

    public void OnDamageTaken() {
        _animator.SetTrigger("Damage");
        health--;

        if (health <= 0) {
            Destruct();
        }
    }

}
