using UnityEngine;

public class LivingEntity : MonoBehaviour {

    public NetworkEntity NetworkEntity { get { return _networkEntity; } }
    public CharacterStats CharacterStats { get { return _characterStats; } }
    public Type EntityType { get { return _type; } }
    public PlayerClass PlayerClass { get { return _playerClass; } }
    public GameObject CharacterObject { get { return _characterObject; } }

    public bool IsDeath { get; set; }

    protected NetworkEntity _networkEntity;
    protected CharacterStats _characterStats;
    protected GameObject _characterObject;

    public enum Type {
        Player,
        BOT,
        Object
    }

    [Header("Initialization")]
    [SerializeField]
    private Type _type;
    [SerializeField]
    protected GameObject _warriorPrefab;
    [SerializeField]
    protected GameObject _archerPrefab;
    [SerializeField]
    protected GameObject _magePrefab;
    [SerializeField]
    protected GameObject _priestPrefab;

    [Header("Debug")]
    protected PlayerClass _playerClass;

}
