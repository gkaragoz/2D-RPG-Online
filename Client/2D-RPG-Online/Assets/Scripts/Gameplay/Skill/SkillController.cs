using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillController : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Skill _skillPrefab;
    [SerializeField]
    private Transform _FXCenterPoint;
    [SerializeField]
    private Transform _FXBasicAtackPoint;

    [Header("Debug")]
    [Utils.ReadOnly]
    [SerializeField]
    private List<Skill> _skills = new List<Skill>();
    [Utils.ReadOnly]
    [SerializeField]
    private PlayerClass _playerClass;

    public void Initialize(PlayerClass playerClass) {
        this._playerClass = playerClass;

        SkillDatabase db = null;

        switch (this._playerClass) {
            case PlayerClass.Warrior:
                db = Resources.Load<SkillDatabase>("ScriptableObjects/Skills/Warrior_SkillDatabase");
                break;
            case PlayerClass.Archer:
                break;
            case PlayerClass.Mage:
                break;
            case PlayerClass.Priest:
                db = Resources.Load<SkillDatabase>("ScriptableObjects/Skills/Priest_SkillDatabase");
                break;
            default:
                break;
        }

        for (int ii = 0; ii < db.skills.Count; ii++) {
            Skill skill = Instantiate(_skillPrefab, transform);
            skill.Initialize(db.skills[ii]);

            _skills.Add(skill);
        }
    }

    public void OnAttack(Skill_SO.Skill_Name skillName, Transform target = null) {
        Skill skill = _skills.Where(s => s.SkillName == skillName).First();

        if (target == null) {
            skill.Run(_FXBasicAtackPoint.position, Skill.VFXType.Action);
        } else {
            skill.Run(_FXBasicAtackPoint.position, target, Skill.VFXType.Action);
        }
    }

    public void OnTakeDamage(Skill_SO.Skill_Name skillName) {
        Skill skill = _skills.Where(s => s.SkillName == skillName).First();
        skill.Run(_FXCenterPoint.position, Skill.VFXType.Effect);
    }

}