using UnityEngine;

public class CharacterStats : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private CharacterStats_SO _characterDefinition_Template;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private CharacterStats_SO _characterDefinition;

    #region Initializations

    private void Awake() {
        if (_characterDefinition_Template != null) {
            _characterDefinition = Instantiate(_characterDefinition_Template);
        }
    }

    #endregion

    public void Initialize(NetworkIdentifier networkObject) {
        _characterDefinition.Initialize(networkObject);
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
        _characterDefinition.AddStatsPoints(amount);
    }

    public void IncreaseStrength() {
        _characterDefinition.IncreaseStrength();
    }

    public void IncreaseDexterity() {
        _characterDefinition.IncreaseDexterity();
    }

    public void IncreaseIntelligence() {
        _characterDefinition.IncreaseIntelligence();
    }

    public void ApplyHealth(float healthAmount) {
        _characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(float manaAmount) {
        _characterDefinition.ApplyMana(manaAmount);
    }

    public void AddAttackDamage(int damageAmount) {
        _characterDefinition.AddAttackDamage(damageAmount);
    }

    public void AddAttackSpeed(int speedAmount) {
        _characterDefinition.AddAttackSpeed(speedAmount);
    }

    public void AddAttackRange(int speedAmount) {
        _characterDefinition.AddAttackRange(speedAmount);
    }

    public void AddExp(int expAmount) {
        _characterDefinition.AddExp(expAmount);
    }

    #endregion

    #region Stat Reducers

    public void TakeDamage(float amount) {
        _characterDefinition.TakeDamage(amount);
    }

    public void TakeMana(float amount) {
        _characterDefinition.TakeMana(amount);
    }

    public void ReduceAttackDamage(int amount) {
        _characterDefinition.ReduceAttackDamage(amount);
    }

    public void ReduceAttackSpeed(int amount) {
        _characterDefinition.ReduceAttackSpeed(amount);
    }

    public void ReduceAttackRange(float amount) {
        _characterDefinition.ReduceAttackRange(amount);
    }

    public void LooseExp(int amount) {
        _characterDefinition.LooseExp(amount);
    }

    #endregion

    #region Reporters

    public string GetName() {
        return _characterDefinition.Name;
    }

    public bool CanIncreaseStrength() {
        return GetStatsPoints() >= _characterDefinition.BaseStrength.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseDexterity() {
        return GetStatsPoints() >= _characterDefinition.BaseDexterity.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseIntelligence() {
        return GetStatsPoints() >= _characterDefinition.BaseIntelligence.RequiredStatsPoints ? true : false;
    }

    public bool HasStatsPoints() {
        return _characterDefinition.StatsPoints > 0 ? true : false;
    }

    public int GetStatsPoints() {
        return _characterDefinition.StatsPoints;
    }

    public int GetBaseStrength() {
        return _characterDefinition.BaseStrength.Value;
    }

    public int GetBaseDexterity() {
        return _characterDefinition.BaseDexterity.Value;
    }

    public int GetBaseIntelligence() {
        return _characterDefinition.BaseIntelligence.Value;
    }

    public float GetMaxHealth() {
        return _characterDefinition.MaxHealth;
    }

    public float GetCurrentHealth() {
        return _characterDefinition.CurrentHealth;
    }

    public float GetMaxMana() {
        return _characterDefinition.MaxMana;
    }

    public float GetCurrentMana() {
        return _characterDefinition.CurrentMana;
    }

    public int GetAttackDamage() {
        return _characterDefinition.AttackDamage;
    }

    public float GetAttackSpeed() {
        return _characterDefinition.AttackSpeed;
    }

    public float GetMovementSpeed() {
        return _characterDefinition.MovementSpeed;
    }

    public float GetAttackRange() {
        return _characterDefinition.AttackRange;
    }

    public int GetLevel() {
        return _characterDefinition.Level;
    }

    public int GetMaxExperience() {
        return _characterDefinition.MaxExperience;
    }

    public int GetCurrentExperience() {
        return _characterDefinition.CurrentExperience;
    }

    #endregion

}
