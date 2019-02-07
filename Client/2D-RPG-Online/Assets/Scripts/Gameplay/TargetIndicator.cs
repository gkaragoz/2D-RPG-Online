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

    public void SetPosition(Transform target, Type type) {
        SetColor(type);

        Show();

        transform.localPosition = target.position;
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
