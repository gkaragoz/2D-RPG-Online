using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorController : MonoBehaviour {

    public enum Type {
        Damage,
        Heal
    }

    public Transform parentTransform;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Transform _target;

    public void CreateDamageIndicator(Type type, string text, Transform location) {
        //DamageIndicator instance = ObjectPooler.instance.SpawnFromPool("DamageIndicator").GetComponent<DamageIndicator>();

        //instance.transform.SetParent(parentTransform, false);
        //instance.SetColor(type);
        //instance.SetText(text);
        //instance.Play();
    }

}
