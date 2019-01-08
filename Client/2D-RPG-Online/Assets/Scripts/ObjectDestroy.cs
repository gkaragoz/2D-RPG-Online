using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour {

    public void Destroy(float time = 0f) {
        Destroy(this.gameObject, time);
    }

}
