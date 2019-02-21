using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorController : MonoBehaviour {

    [SerializeField]
    private Transform _spawnPosition;

    public enum Type {
        Damage,
        Heal
    }

    public void CreateDamageIndicator(Type type, string text) {
        DamageIndicator instance = ObjectPooler.instance.SpawnFromPool("DamageIndicator").GetComponent<DamageIndicator>();

        instance.SetPosition(_spawnPosition.position);
        instance.SetColor(type);
        instance.SetText(text);
        instance.Play();
    }

}
