using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Character Stats", menuName = "Scriptable Objects/Character Stats")]
public class CharacterStats_SO : ScriptableObject {

    #region Base Stats Class

    [System.Serializable]
    public class BaseStats {

        [SerializeField]
        private int _value;
        [SerializeField]
        private int _requiredStatsPoints = 1;

        public int Value {
            get {
                return _value;
            }

            private set {
                _value = value;
            }
        }

        public int RequiredStatsPoints {
            get {
                return _requiredStatsPoints;
            }

            private set {
                _requiredStatsPoints = value;
            }
        }

        public BaseStats(int value, int requiredStatsPoints = 1) {
            this.Value = value;
            this.RequiredStatsPoints = requiredStatsPoints;
        }

        public void Increase() {
            Value++;

            SetRequiredStatsPoint();
        }

        private void SetRequiredStatsPoint() {
            int baseLimit = 50;
            int frequency = 10;

            if (Value < baseLimit) {
                RequiredStatsPoints = 1;
            } else {
                RequiredStatsPoints = (int)Mathf.Abs(((Value - baseLimit)) / frequency) + 2;
            }
        }
    }

    #endregion

    #region Properties

    public bool isPlayer = false;

    [SerializeField]
    private string _name = string.Empty;

    [SerializeField]
    private int _statsPoints = 0;

    [SerializeField]
    private BaseStats _baseStrength;
    [SerializeField]
    private BaseStats _baseDexterity;
    [SerializeField]
    private BaseStats _baseIntelligence;

    [SerializeField]
    private float _maxHealth = 0;
    [SerializeField]
    private float _currentHealth = 0;

    [SerializeField]
    private float _maxMana = 0;
    [SerializeField]
    private float _currentMana = 0;

    [SerializeField]
    private int _attackDamage = 5;
    [SerializeField]
    private float _attackSpeed = 1.0f;
    [SerializeField]
    private float _movementSpeed = 2;
    [SerializeField]
    private float _attackRange = 1;

    [SerializeField]
    private int _level = 0;
    [SerializeField]
    private int _maxExperience = 0;
    [SerializeField]
    private int _currentExperience = 0;

    #endregion

    #region Getter Setters

    public string Name {
        get {
            return _name;
        }

        set {
            _name = value;
        }
    }

    public int StatsPoints {
        get {
            return _statsPoints;
        }

        set {
            _statsPoints = value;
        }
    }

    public BaseStats BaseStrength {
        get {
            return _baseStrength;
        }

        set {
            _baseStrength = value;
        }
    }

    public BaseStats BaseDexterity {
        get {
            return _baseDexterity;
        }

        set {
            _baseDexterity = value;
        }
    }

    public BaseStats BaseIntelligence {
        get {
            return _baseIntelligence;
        }

        set {
            _baseIntelligence = value;
        }
    }

    public float MaxHealth {
        get {
            return _maxHealth;
        }

        set {
            _maxHealth = value;
        }
    }

    public float CurrentHealth {
        get {
            return _currentHealth;
        }

        set {
            _currentHealth = value;
        }
    }

    public float MaxMana {
        get {
            return _maxMana;
        }

        set {
            _maxMana = value;
        }
    }

    public float CurrentMana {
        get {
            return _currentMana;
        }

        set {
            _currentMana = value;
        }
    }

    public int AttackDamage {
        get {
            return _attackDamage;
        }

        set {
            _attackDamage = value;
            if (_attackDamage < 0) {
                _attackDamage = 0;
            }
        }
    }

    public float AttackSpeed {
        get {
            return _attackSpeed;
        }

        set {
            _attackSpeed = value;
            if (_attackSpeed < 0) {
                _attackSpeed = 0;
            }
        }
    }

    public float AttackRange {
        get {
            return _attackRange;
        }

        set {
            _attackRange = value;
            if (_attackRange < 0) {
                _attackRange = 0;
            }
        }
    }

    public float MovementSpeed {
        get {
            return _movementSpeed;
        }

        set {
            _movementSpeed = value;
            if (_movementSpeed < 0) {
                _movementSpeed = 0;
            }
        }
    }

    public int Level {
        get {
            return _level;
        }

        set {
            _level = value;
        }
    }

    public int MaxExperience {
        get {
            return _maxExperience;
        }

        set {
            _maxExperience = value;
        }
    }

    public int CurrentExperience {
        get {
            return _currentExperience;
        }

        set {
            _currentExperience = value;
        }
    }

    #endregion

    #region Stat Increasers

    public void AddStatsPoints(int statsPointsAmount) {
        StatsPoints += statsPointsAmount;
    }

    public void IncreaseStrength() {
        if (StatsPoints >= BaseStrength.RequiredStatsPoints) {
            SpendStatsPoints(BaseStrength.RequiredStatsPoints);
            BaseStrength.Increase();
        } 
    }

    public void IncreaseDexterity() {
        if (StatsPoints >= BaseDexterity.RequiredStatsPoints) {
            SpendStatsPoints(BaseDexterity.RequiredStatsPoints);
            BaseDexterity.Increase();
        }
    }

    public void IncreaseIntelligence() {
        if (StatsPoints >= BaseIntelligence.RequiredStatsPoints) {
            SpendStatsPoints(BaseIntelligence.RequiredStatsPoints);
            BaseIntelligence.Increase();
        }
    }

    public void ApplyHealth(float healthAmount) {
        if ((CurrentHealth + healthAmount) > MaxHealth) {
            CurrentHealth = MaxHealth;
        } else {
            CurrentHealth += healthAmount;
        }
    }

    public void ApplyMana(float manaAmount) {
        if ((CurrentMana + manaAmount) > MaxMana) {
            CurrentMana = MaxMana;
        } else {
            CurrentMana += manaAmount;
        }
    }

    public void AddAttackDamage(int damageAmount) {
        AttackDamage += damageAmount;
    }

    public void AddAttackSpeed(float speedAmount) {
        AttackSpeed += speedAmount;
    }

    public void AddAttackRange(float rangeAmount) {
        AttackRange += rangeAmount;
    }

    public void AddExp(int expAmount) {
        if (CurrentExperience + expAmount >= MaxExperience) {
            int needExpAmount = MaxExperience - CurrentExperience;
            int remainingExpAmount = expAmount - needExpAmount;

            LevelUp();

            if (remainingExpAmount > 0) {
                AddExp(remainingExpAmount);
            } else {
                CurrentExperience += needExpAmount;
            }
        } else {
            CurrentExperience += expAmount;
        }
    }

    #endregion

    #region Stat Reducers

    public void SpendStatsPoints(int amount) {
        StatsPoints -= amount;
    }

    public void TakeDamage(float amount) {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0) {
            CurrentHealth = 0;
            Debug.Log("Death");
        }
    }

    public void TakeMana(float amount) {
        CurrentMana -= amount;

        if (CurrentMana <= 0) {
            CurrentMana = 0;
        }
    }

    public void ReduceAttackDamage(int damageAmount) {
        AttackDamage -= damageAmount;
    }

    public void ReduceAttackSpeed(float speedAmount) {
        AttackSpeed -= speedAmount;
    }

    public void ReduceAttackRange(float rangeAmount) {
        AttackRange -= rangeAmount;
    }

    public void LooseExp(int expAmount) {
        CurrentExperience -= expAmount;

        if (CurrentExperience <= 0) {
            CurrentExperience = 0;
        }
    }

    #endregion

    #region Character Level Up and Death

    public void Death() {
        //Call to Game Manager for Death State to trigger respawn.
        //Display death visualization.
    }

    public void Initialize(NetworkIdentifier networkObject) {
        //this.StatsPoints = AccountManager.instance.SelectedCharacterStatsPoint;
        this.Name = AccountManager.instance.SelectedCharacterName;

        this.BaseStrength = new BaseStats(networkObject.PlayerData.Strength);
        this.BaseDexterity = new BaseStats(networkObject.PlayerData.Dexterity);
        this.BaseIntelligence = new BaseStats(networkObject.PlayerData.Intelligence);

        this.CurrentHealth = networkObject.PlayerData.CurrentHp;
        this.MaxHealth = networkObject.PlayerData.MaxHp;
        this.CurrentMana = networkObject.PlayerData.CurrentMana;
        this.MaxMana = networkObject.PlayerData.MaxMana;

        this.AttackDamage = networkObject.PlayerData.AttackDamage;
        this.AttackSpeed = networkObject.PlayerData.AttackSpeed;
        this.AttackRange = networkObject.PlayerData.AttackRange;
        this.MovementSpeed = networkObject.PlayerData.MoveSpeed;

        //this.Level = level;
        this.MaxExperience = 10 + (Level * 10) + (Level + 1) ^ 3;
        //this.MaxExperience = maxExperience;
        this.CurrentExperience = 0;
    }

    public void LevelUp() {
        StatsPoints += 12;
        Level++;

        CurrentExperience = 0;
        MaxExperience = 10 + (Level * 10) + (int)Mathf.Pow(Level + 1, 3);

        //LogManager.instance.AddLog("Level up! Your level is: " + Level, Log.Type.Loot);
        //LogManager.instance.AddLog("You have unspend " + StatsPoints + " stats points.", Log.Type.Loot);
    }

    #endregion
}
