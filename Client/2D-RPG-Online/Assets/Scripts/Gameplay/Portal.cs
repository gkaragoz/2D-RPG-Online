using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Transform spawnedTransform;
    public Transform spawnPoint;

    public void Init(Transform spawnedTransform) {
        this.spawnedTransform = spawnedTransform;
    }

    public void Spawn() {
        spawnedTransform = Instantiate(spawnedTransform);
        spawnedTransform.position = spawnPoint.position;
    }

}
