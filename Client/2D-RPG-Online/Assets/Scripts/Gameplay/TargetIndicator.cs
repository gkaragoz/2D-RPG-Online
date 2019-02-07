using UnityEngine;

public class TargetIndicator : Menu {

    #region Singleton

    public static TargetIndicator instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [SerializeField]
    private RectTransform _canvasRect;

    public void SetPosition(Transform target) {
        Show();

        transform.localPosition = target.position;
    }

}
