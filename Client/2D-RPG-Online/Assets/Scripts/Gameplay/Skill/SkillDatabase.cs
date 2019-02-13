using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillDatabase : MonoBehaviour {

    #region Singleton

    public static SkillDatabase instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    private List<Skill_SO> _allSkills = new List<Skill_SO>();

    private SkillDatabase_SO _warriorSkillsDB;
    private SkillDatabase_SO _archerSkillsDB;
    private SkillDatabase_SO _mageSkillsDB;
    private SkillDatabase_SO _priestSkillsDB;

    private void Start() {
        _warriorSkillsDB = Resources.Load<SkillDatabase_SO>("ScriptableObjects/Skills/Warrior_SkillDatabase");
        _priestSkillsDB = Resources.Load<SkillDatabase_SO>("ScriptableObjects/Skills/Priest_SkillDatabase");

        _allSkills.AddRange(_warriorSkillsDB.skills);
        _allSkills.AddRange(_priestSkillsDB.skills);
    }

    public Skill_SO GetSkill(Skill_SO.Skill_Name skillName) {
        return _allSkills.Where(s => s._skillName == skillName).First();
    }

    public Skill_SO GetBasicAttackSkill(PlayerClass playerClass) {
        switch (playerClass) {
            case PlayerClass.Warrior:
                return _allSkills.Where(s => s._skillName == Skill_SO.Skill_Name.Warrior_BasicAttack).First();
            case PlayerClass.Archer:
                //_skills.Where(s => s._skillName == Skill_SO.Skill_Name.Archer_BasicAttack).First();
                return null;
            case PlayerClass.Mage:
                //_skills.Where(s => s._skillName == Skill_SO.Skill_Name.Mage_BasicAttack).First();
                return null;
            case PlayerClass.Priest:
                return _allSkills.Where(s => s._skillName == Skill_SO.Skill_Name.Priest_BasicAttack).First();
            default:
                return null;
        }
    }

    public Skill_SO.Skill_Name GetSkillName(PlayerClass playerClass) {
        Skill_SO.Skill_Name skillName = Skill_SO.Skill_Name.None;

        switch (playerClass) {
            case PlayerClass.Warrior:
                skillName = Skill_SO.Skill_Name.Warrior_BasicAttack;
                break;
            case PlayerClass.Archer:
                skillName = Skill_SO.Skill_Name.Archer_BasicAttack;
                break;
            case PlayerClass.Mage:
                skillName = Skill_SO.Skill_Name.Mage_BasicAttack;
                break;
            case PlayerClass.Priest:
                skillName = Skill_SO.Skill_Name.Priest_BasicAttack;
                break;
            default:
                skillName = Skill_SO.Skill_Name.None;
                Debug.Log("Skill not found!");
                break;
        }

        return skillName;
    }

}
