using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, ILivingEntity {

    public Action onDeathEvent;

    [Header("Initialization")]
    [SerializeField]
    private LivingEntity_SO _livingEntityDefinition_Template;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private LivingEntity_SO _livingEntityDefinition;

    public enum Type {
        Player,
        BOT,
        Object
    }

    public bool IsDeath {
        get {
            return _livingEntityDefinition.isDeath;
        }
        set {
            _livingEntityDefinition.isDeath = value;

            if (value == true) {
                onDeathEvent?.Invoke();
            }
        }
    }

    public Type EntityType { get { return _type; } }

    [SerializeField]
    private Type _type;

    public virtual void Awake() {
        if (_livingEntityDefinition_Template != null) {
            _livingEntityDefinition = Instantiate(_livingEntityDefinition_Template);
        }
    }

    public virtual void TakeDamage(int damage) {

    }

    public virtual void OnDeath() {

    }

    public virtual void Attack() {

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
