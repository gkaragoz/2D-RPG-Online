using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Animator animator;
    public Transform spawnedTransform;

    public void Init(Transform spawnedTransform) {
        this.spawnedTransform = spawnedTransform;
    }

    public void Spawn() {
        spawnedTransform = Instantiate(spawnedTransform);
        animator.SetTrigger("Move");
    }

    private void Update() {
        if (spawnedTransform != null) {
            spawnedTransform.position = animator.transform.position;
        }
    }

}
