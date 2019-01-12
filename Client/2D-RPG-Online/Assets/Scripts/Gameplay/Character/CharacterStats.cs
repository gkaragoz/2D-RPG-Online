using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private CharacterStats_SO _characterDefinition_Template;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    public CharacterStats_SO characterDefinition;

    #region Initializations

    private void Awake() {
        if (_characterDefinition_Template != null) {
            characterDefinition = Instantiate(_characterDefinition_Template);
        }
        if (characterDefinition.isPlayer) {
            characterDefinition.SetLevel(0);
        }
    }

    #endregion

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            //LogManager.instance.AddLog("You gained 1 exp!", Log.Type.Exp);
            AddExp(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            //LogManager.instance.AddLog("You gained 10 exp!", Log.Type.Exp);
            AddExp(10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            //LogManager.instance.AddLog("You gained 100 exp!", Log.Type.Exp);
            AddExp(100);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            //LogManager.instance.AddLog("You gained 1000 exp!", Log.Type.Exp);
            AddExp(1000);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            //LogManager.instance.AddLog("You gained 10000 exp!", Log.Type.Exp);
            AddExp(10000);
        }
    }

    #region Stat Increasers
    public void AddStatsPoints(int amount) {
        characterDefinition.AddStatsPoints(amount);
    }

    public void IncreaseStrength() {
        characterDefinition.IncreaseStrength();
    }

    public void IncreaseDexterity() {
        characterDefinition.IncreaseDexterity();
    }

    public void IncreaseConstitution() {
        characterDefinition.IncreaseConstitution();
    }

    public void IncreaseAgility() {
        characterDefinition.IncreaseAgility();
    }

    public void IncreaseIntelligence() {
        characterDefinition.IncreaseIntelligence();
    }

    public void IncreaseCharisma() {
        characterDefinition.IncreaseCharisma();
    }

    public void ApplyHealth(float healthAmount) {
        characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyStamina(float staminaAmount) {
        characterDefinition.ApplyStamina(staminaAmount);
    }

    public void AddWeight(float weightAmount) {
        characterDefinition.AddWeight(weightAmount);
    }

    public void AddExp(int expAmount) {
        characterDefinition.AddExp(expAmount);
    }

    #endregion

    #region Stat Reducers

    public void TakeDamage(float amount) {
        characterDefinition.TakeDamage(amount);
    }

    public void TakeStamina(float amount) {
        characterDefinition.TakeStamina(amount);
    }

    public void ReleaseWeight(float amount) {
        characterDefinition.ReleaseWeight(amount);
    }

    public void LooseExp(int amount) {
        characterDefinition.LooseExp(amount);
    }

    #endregion

    #region Reporters
    public bool CanCarryThisWeight(float weight) {
        if (weight + GetCurrentWeight() > GetMaxWeight()) {
            return false;
        } else {
            return true;
        }
    }

    public bool CanIncreaseStrength() {
        return GetStatsPoints() >= characterDefinition.BaseStrength.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseDexterity() {
        return GetStatsPoints() >= characterDefinition.BaseDexterity.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseConstitution() {
        return GetStatsPoints() >= characterDefinition.BaseConstitution.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseAgility() {
        return GetStatsPoints() >= characterDefinition.BaseAgility.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseIntelligence() {
        return GetStatsPoints() >= characterDefinition.BaseIntelligence.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseCharisma() {
        return GetStatsPoints() >= characterDefinition.BaseCharisma.RequiredStatsPoints ? true : false;
    }

    public bool HasStatsPoints() {
        return characterDefinition.StatsPoints > 0 ? true : false;
    }

    public int GetStatsPoints() {
        return characterDefinition.StatsPoints;
    }

    public int GetBaseStrength() {
        return characterDefinition.BaseStrength.Value;
    }

    public int GetBaseDexterity() {
        return characterDefinition.BaseDexterity.Value;
    }

    public int GetBaseConstitution() {
        return characterDefinition.BaseConstitution.Value;
    }

    public int GetBaseAgility() {
        return characterDefinition.BaseAgility.Value;
    }

    public int GetBaseIntelligence() {
        return characterDefinition.BaseIntelligence.Value;
    }

    public int GetBaseCharisma() {
        return characterDefinition.BaseCharisma.Value;
    }

    public float GetMaxHealth() {
        return characterDefinition.MaxHealth;
    }

    public float GetCurrentHealth() {
        return characterDefinition.CurrentHealth;
    }

    public float GetMaxStamina() {
        return characterDefinition.MaxStamina;
    }

    public float GetCurrentStamina() {
        return characterDefinition.CurrentStamina;
    }

    public float GetMaxWeight() {
        return characterDefinition.MaxWeight;
    }

    public float GetCurrentWeight() {
        return characterDefinition.CurrentWeight;
    }

    public float GetSpeed() {
        return characterDefinition.Speed;
    }
    
    public float GetAngularSpeed() {
        return characterDefinition.AngularSpeed;
    }

    public float GetAcceleration() {
        return characterDefinition.Acceleration;
    }

    public int GetLevel() {
        return characterDefinition.Level;
    }

    public int GetMaxExperience() {
        return characterDefinition.MaxExperience;
    }

    public int GetCurrentExperience() {
        return characterDefinition.CurrentExperience;
    }

    public float GetFame() {
        return characterDefinition.Fame;
    }

    public float GetHonor() {
        return characterDefinition.Honor;
    }

    #endregion

}
