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
    }

    #endregion

    public void Initialize(NetworkIdentifier networkObject) {
        characterDefinition.Initialize(networkObject);
    }

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

    public void IncreaseIntelligence() {
        characterDefinition.IncreaseIntelligence();
    }

    public void ApplyHealth(float healthAmount) {
        characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(float manaAmount) {
        characterDefinition.ApplyMana(manaAmount);
    }

    public void AddAttackDamage(int damageAmount) {
        characterDefinition.AddAttackDamage(damageAmount);
    }

    public void AddAttackSpeed(int speedAmount) {
        characterDefinition.AddAttackSpeed(speedAmount);
    }

    public void AddAttackRange(int speedAmount) {
        characterDefinition.AddAttackRange(speedAmount);
    }

    public void AddExp(int expAmount) {
        characterDefinition.AddExp(expAmount);
    }

    #endregion

    #region Stat Reducers

    public void TakeDamage(float amount) {
        characterDefinition.TakeDamage(amount);
    }

    public void TakeMana(float amount) {
        characterDefinition.TakeMana(amount);
    }

    public void ReduceAttackDamage(int amount) {
        characterDefinition.ReduceAttackDamage(amount);
    }

    public void ReduceAttackSpeed(int amount) {
        characterDefinition.ReduceAttackSpeed(amount);
    }

    public void ReduceAttackRange(float amount) {
        characterDefinition.ReduceAttackRange(amount);
    }

    public void LooseExp(int amount) {
        characterDefinition.LooseExp(amount);
    }

    #endregion

    #region Reporters

    public bool CanIncreaseStrength() {
        return GetStatsPoints() >= characterDefinition.BaseStrength.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseDexterity() {
        return GetStatsPoints() >= characterDefinition.BaseDexterity.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseIntelligence() {
        return GetStatsPoints() >= characterDefinition.BaseIntelligence.RequiredStatsPoints ? true : false;
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

    public int GetBaseIntelligence() {
        return characterDefinition.BaseIntelligence.Value;
    }

    public float GetMaxHealth() {
        return characterDefinition.MaxHealth;
    }

    public float GetCurrentHealth() {
        return characterDefinition.CurrentHealth;
    }

    public float GetMaxMana() {
        return characterDefinition.MaxMana;
    }

    public float GetCurrentMana() {
        return characterDefinition.CurrentMana;
    }

    public int GetAttackDamage() {
        return characterDefinition.AttackDamage;
    }

    public float GetAttackSpeed() {
        return characterDefinition.AttackSpeed;
    }

    public float GetMovementSpeed() {
        return characterDefinition.MovementSpeed;
    }

    public float GetAttackRange() {
        return characterDefinition.AttackRange;
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

    #endregion

}
