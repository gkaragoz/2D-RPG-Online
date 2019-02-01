using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationTargetFrame : MonoBehaviour {

    [Range(1, 60)]
    public int targetFrameRate = 30;

    private void Start() {
        Application.targetFrameRate = targetFrameRate; 
    }

}
