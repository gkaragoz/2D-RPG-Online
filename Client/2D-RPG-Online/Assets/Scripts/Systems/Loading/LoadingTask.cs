using UnityEngine;

[System.Serializable]
public class LoadingTask {

    public string Name { get { return _taskName; } }
    public bool IsCompleted { get { return _isCompleted; } }

    [SerializeField]
    private bool _isCompleted;
    [SerializeField]
    private string _taskName;

    public void Complete() {
        if (!_isCompleted) {
            _isCompleted = true;
            LoadingManager.instance.Progress(this);
        }
    }

}