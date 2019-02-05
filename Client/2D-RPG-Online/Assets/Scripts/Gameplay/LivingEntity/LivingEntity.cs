using UnityEngine;

public class LivingEntity : MonoBehaviour, ILivingEntity {

    public NetworkEntity NetworkEntity { get { return _networkEntity; } }
    public CharacterStats CharacterStats { get { return _characterStats; } }

    public bool IsDeath { get; set; }

    protected NetworkEntity _networkEntity;
    protected CharacterStats _characterStats;

    public enum Type {
        Player,
        BOT,
        Object
    }

    public Type EntityType { get { return _type; } }

    [Header("Initialization")]
    [SerializeField]
    private Type _type;

    public virtual void TakeDamage(int damage) {

    }

    public virtual void Die() {

    }

    public virtual void Attack() {

    }

    public virtual void AttackAnimation(Vector3 direction) {

    }

    public virtual void MoveByInput() {

    }

    public virtual void MoveToPosition(Vector3 position) {

    }

    public virtual void Stop() {

    }

    public virtual void Rotate() {

    }

    public virtual void Destroy() {

    }

}
