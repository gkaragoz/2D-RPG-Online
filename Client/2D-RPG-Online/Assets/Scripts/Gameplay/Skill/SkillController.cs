using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Skill _skillPrefab;
    [SerializeField]
    private Transform _FXCenterPoint;
    [SerializeField]
    private Transform _FXBasicAtackPoint;

    [Utils.ReadOnly]
    [SerializeField]
    private PlayerClass _playerClass;

    public void Initialize(PlayerClass playerClass) {
        this._playerClass = playerClass;
    }

    public void OnAttack(Skill_SO skillSO, Transform target = null) {
        if (skillSO == null) {
            Debug.LogError("Skill not found!");
            return;
        }

        Skill skill = Instantiate(_skillPrefab);
        skill.Initialize(skillSO);

        if (target == null) {
            skill.Run(_FXBasicAtackPoint.position, Skill.VFXType.Action);
        } else {
            skill.Run(_FXBasicAtackPoint.position, target, Skill.VFXType.Action);
        }
    }

    public void OnTakeDamage(Skill_SO skillSO) {
        if (skillSO == null) {
            Debug.LogError("Skill not found!");
            return;
        }

        Skill skill = Instantiate(_skillPrefab);
        skill.Initialize(skillSO);

        skill.Run(_FXCenterPoint.position, Skill.VFXType.Effect);
    }

}