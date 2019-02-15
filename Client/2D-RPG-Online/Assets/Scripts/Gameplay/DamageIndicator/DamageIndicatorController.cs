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
    private List<DamageIndicator> _damageIndicators = new List<DamageIndicator>();
    [SerializeField]
    [Utils.ReadOnly]
    private Transform _target;

    private void Start() {
        SceneController.instance.onSceneLoaded += OnSceneLoaded;
    }

    public void CreateDamageIndicator(Type type, string text, Transform location) {
        DamageIndicator instance = ObjectPooler.instance.SpawnFromPool("DamageIndicator").GetComponent<DamageIndicator>();

        instance.transform.SetParent(parentTransform, false);
        instance.SetColor(type);
        instance.SetText(text);
        instance.Play();
    }

    private void OnSceneLoaded() {
        //GameObject[] damageIndicatorObjects = GameObject.FindGameObjectsWithTag("DamageIndicator");

        //for (int ii = 0; ii < damageIndicatorObjects.Length; ii++) {
        //    _damageIndicators.Add(damageIndicatorObjects[ii].GetComponent<DamageIndicator>());
        //    _damageIndicators[ii].Hide();
        //}
    }

}
