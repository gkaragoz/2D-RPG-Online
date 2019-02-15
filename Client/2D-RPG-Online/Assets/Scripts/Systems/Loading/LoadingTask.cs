using UnityEngine;

[System.Serializable]
public class LoadingTask {

    public string Name { get { return _taskName; } }

    [SerializeField]
    private string _taskName;

    public void Complete() {
        LoadingManager.instance.Progress(this);
    }

}