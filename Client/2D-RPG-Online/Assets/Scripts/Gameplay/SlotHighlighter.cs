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

    public void SetPosition(Transform target) {
        _spotLight.gameObject.SetActive(true);
        transform.localPosition = target.position;
    }

}
