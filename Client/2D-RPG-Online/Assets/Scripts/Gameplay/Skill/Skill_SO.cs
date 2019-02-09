using UnityEngine;

[CreateAssetMenu(fileName = "Skill Name", menuName = "Scriptable Objects/Skill")]
public class Skill_SO : ScriptableObject {

    public enum Skill_Name {
        BasicAttack
    }

    public enum Skill_Type {
        Attack,
        Buff
    }

    public enum Skill_Range {
        Melee,
        Ranged
    }

    public enum Skill_Behaviour {
        Immidiate,
        Focused
    }

    public Skill_Name _skillName;
    public Skill_Type _skillType;
    public Skill_Range _skillRange;
    public Skill_Behaviour _skillBehaviour;
    public ParticleSystem _VFX_ActionPrefab;
    public ParticleSystem _VFX_EffectPrefab;

}
