﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;

    public Vector3 offset;

    private void LateUpdate() {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }
}
