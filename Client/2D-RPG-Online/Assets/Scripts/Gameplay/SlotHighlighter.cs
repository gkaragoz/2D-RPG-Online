using UnityEngine;

public class SlotHighlighter : MonoBehaviour {

    #region Singleton

    public static SlotHighlighter instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [Header("Initialization")]
    [SerializeField]
    private Light _spotLight;

    private void Start() {
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;
    }

    public void OnLoadingCompleted() {
        if (SceneController.instance.State == SceneController.STATE.Gameplay) {
            _spotLight.gameObject.SetActive(false);
        }
    }

    public void SetPosition(Transform target) {
        _spotLight.gameObject.SetActive(true);
        transform.localPosition = target.position;
    }

}
