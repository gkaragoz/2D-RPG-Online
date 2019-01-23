using System;
using UnityEngine;

public class LoadingTask : MonoBehaviour {

    public Action taskAction;

    private void Start() {
        taskAction = new Action(LoadingManager.instance.Progress);
    }

    public void Invoke() {
        taskAction?.Invoke();
    }

}