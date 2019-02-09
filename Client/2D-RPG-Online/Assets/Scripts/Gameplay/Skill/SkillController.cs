using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillController : MonoBehaviour {

    [Header("Debug")]
    [Utils.ReadOnly]
    [SerializeField]
    private List<Skill> _skills = new List<Skill>();
    [SerializeField]
    [Utils.ReadOnly]
    private PlayerClass _playerClass;

    public void Initialize(PlayerClass playerClass, Skill[] skills) {
        this._playerClass = playerClass;

        for (int ii = 0; ii < skills.Length; ii++) {
            _skills.Add(skills[ii]);
        }
    }

    public void OnAttack(Transform target) {
        switch (_playerClass) {
            case PlayerClass.Warrior:
                _skills.Where(skill => skill.SkillName == Skill_SO.Skill_Name.BasicAttack).First().PlayAction(target.localPosition, target.localRotation, 0.25f);
                break;
            case PlayerClass.Archer:
                break;
            case PlayerClass.Mage:
                break;
            case PlayerClass.Priest:
                break;
            default:
                break;
        }
    }
    
}