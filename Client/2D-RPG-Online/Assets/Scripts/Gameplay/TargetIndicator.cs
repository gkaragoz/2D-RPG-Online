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

    public enum Type {
        Enemy,
        Friend,
        Myself,
        CharacterSelection
    }

    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _friendColor;
    [SerializeField]
    private Color _myselfColor;
    [SerializeField]
    private Color _characterSelectionColor;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Transform _target;

    private void Start() {
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;
    }

    private void Update() {
        if (this._target != null && isOpen) {
            transform.localPosition = this._target.position;
        }
    }

    public void OnLoadingCompleted() {
        Hide();
    }

    public void SetPosition(Transform target, Type type) {
        this._target = target;

        SetColor(type);

        Show();

        transform.localPosition = this._target.position;
    }

    private void SetColor(Type type) {
        switch (type) {
            case Type.Enemy:
                _spriteRenderer.color = _enemyColor;
                break;
            case Type.Friend:
                _spriteRenderer.color = _friendColor;
                break;
            case Type.Myself:
                _spriteRenderer.color = _myselfColor;
                break;
            case Type.CharacterSelection:
                _spriteRenderer.color = _characterSelectionColor;
                break;
            default:
                break;
        }
    }

}
