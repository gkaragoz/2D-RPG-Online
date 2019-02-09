using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Database", menuName = "Scriptable Objects/Skill Database")]
public class SkillDatabase : ScriptableObject {

    public List<Skill_SO> skills = new List<Skill_SO>();

}