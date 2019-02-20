using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorController : MonoBehaviour {

    public enum Type {
        Damage,
        Heal
    }

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Transform _target;

    public void CreateDamageIndicator(Type type, string text, Vector3 position) {
        DamageIndicator instance = ObjectPooler.instance.SpawnFromPool("DamageIndicator", position, Quaternion.identity).GetComponent<DamageIndicator>();

        instance.SetColor(type);
        instance.SetText(text);
        instance.Play();
    }

}
