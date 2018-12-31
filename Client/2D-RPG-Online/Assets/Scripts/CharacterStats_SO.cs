using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Character Stats", menuName = "Character/Character Stats")]
public class CharacterStats_SO : ScriptableObject {

    #region Events

    public delegate void CurrentExperienceValueChanged();
    public event CurrentExperienceValueChanged onCurrentExperienceValueChanged;

    public delegate void MaxExperienceValueChanged();
    public event MaxExperienceValueChanged onMaxExperienceValueChanged;

    public delegate void LevelValueChanged();
    public event LevelValueChanged onLevelValueChanged;

    #endregion

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
    private int _statsPoints = 0;

    [SerializeField]
    private BaseStats _baseStrength;
    [SerializeField]
    private BaseStats _baseDexterity;
    [SerializeField]
    private BaseStats _baseConstitution;
    [SerializeField]
    private BaseStats _baseAgility;
    [SerializeField]
    private BaseStats _baseIntelligence;
    [SerializeField]
    private BaseStats _baseCharisma;

    [SerializeField]
    private float _currentHealth = 0;

    [SerializeField]
    private float _currentStamina = 0;

    [SerializeField]
    private float _currentWeight = 0;

    [SerializeField]
    private float _speed = 2;
    [SerializeField]
    private float _angularSpeed = 999;
    [SerializeField]
    private float _acceleration = 8;

    [SerializeField]
    private int _level = 0;
    [SerializeField]
    private int _maxExperience = 0;
    [SerializeField]
    private int _currentExperience = 0;

    [SerializeField]
    private float _fame = 0;
    [SerializeField]
    private float _honor = 0;

    #endregion

    #region Getter Setters

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

    public BaseStats BaseConstitution {
        get {
            return _baseConstitution;
        }

        set {
            _baseConstitution = value;
        }
    }

    public BaseStats BaseAgility {
        get {
            return _baseAgility;
        }

        set {
            _baseAgility = value;
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

    public BaseStats BaseCharisma {
        get {
            return _baseCharisma;
        }

        set {
            _baseCharisma = value;
        }
    }

    public float MaxHealth {
        get {
            return BaseStrength.Value * 5;
        }
    }

    public float CurrentHealth {
        get {
            if (_currentHealth > MaxHealth)
                return MaxHealth;
            else
                return _currentHealth;
        }

        set {
            _currentHealth = value;
        }
    }

    public float MaxStamina {
        get {
            return BaseConstitution.Value * 5;
        }
    }

    public float CurrentStamina {
        get {
            if (_currentStamina > MaxStamina)
                return MaxStamina;
            else
                return _currentStamina;
        }

        set {
            _currentStamina = value;
        }
    }

    public float MaxWeight {
        get {
            return BaseStrength.Value;
        }
    }

    public float CurrentWeight {
        get {
            return _currentWeight;
        }

        set {
            _currentWeight = value;
        }
    }

    public float Speed {
        get {
            return _speed;
        }

        set {
            _speed = value;
            if (_speed < 0) {
                _speed = 0;
            }
        }
    }

    public float AngularSpeed {
        get {
            return _angularSpeed;
        }

        set {
            _angularSpeed = value;
            if (_angularSpeed < 0) {
                _angularSpeed = 0;
            }
        }
    }

    public float Acceleration {
        get {
            return _acceleration;
        }

        set {
            _acceleration = value;
            if (_acceleration < 0) {
                _acceleration = 0;
            }
        }
    }

    public int Level {
        get {
            return _level;
        }

        set {
            _level = value;

            if (onLevelValueChanged != null) {
                onLevelValueChanged.Invoke();
            }
        }
    }

    public int MaxExperience {
        get {
            return _maxExperience;
        }

        set {
            _maxExperience = value;

            if (onMaxExperienceValueChanged != null) {
                onMaxExperienceValueChanged.Invoke();
            }
        }
    }

    public int CurrentExperience {
        get {
            return _currentExperience;
        }

        set {
            _currentExperience = value;

            if (onCurrentExperienceValueChanged != null) {
                onCurrentExperienceValueChanged.Invoke();
            }
        }
    }

    public float Fame {
        get {
            return _fame;
        }

        set {
            _fame = value;
        }
    }

    public float Honor {
        get {
            return _honor;
        }

        set {
            _honor = value;
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

    public void IncreaseConstitution() {
        if (StatsPoints >= BaseConstitution.RequiredStatsPoints) {
            SpendStatsPoints(BaseConstitution.RequiredStatsPoints);
            BaseConstitution.Increase();
        }
    }

    public void IncreaseAgility() {
        if (StatsPoints >= BaseAgility.RequiredStatsPoints) {
            SpendStatsPoints(BaseAgility.RequiredStatsPoints);
            BaseAgility.Increase();
        }
    }

    public void IncreaseIntelligence() {
        if (StatsPoints >= BaseIntelligence.RequiredStatsPoints) {
            SpendStatsPoints(BaseIntelligence.RequiredStatsPoints);
            BaseIntelligence.Increase();
        }
    }

    public void IncreaseCharisma() {
        if (StatsPoints >= BaseCharisma.RequiredStatsPoints) {
            SpendStatsPoints(BaseCharisma.RequiredStatsPoints);
            BaseCharisma.Increase();
        }
    }

    public void ApplyHealth(float healthAmount) {
        if ((CurrentHealth + healthAmount) > MaxHealth) {
            CurrentHealth = MaxHealth;
        } else {
            CurrentHealth += healthAmount;
        }
    }

    public void ApplyStamina(float staminaAmount) {
        if ((CurrentStamina + staminaAmount) > MaxStamina) {
            CurrentStamina = MaxStamina;
        } else {
            CurrentStamina += staminaAmount;
        }
    }

    public void AddWeight(float weightAmount) {
        if ((CurrentWeight + weightAmount) > MaxWeight) {
            CurrentWeight = MaxWeight;
        } else {
            CurrentWeight += weightAmount;
        }
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
            //Death();
        }
    }

    public void TakeStamina(float amount) {
        CurrentStamina -= amount;

        if (CurrentStamina <= 0) {
            CurrentStamina = 0;
        }
    }

    public void ReleaseWeight(float amount) {
        CurrentWeight -= amount;

        if (CurrentWeight <= 0) {
            CurrentWeight = 0;
        }
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

    public void SetLevel(int level) {
        StatsPoints = 0;

        BaseStrength = new BaseStats(40);
        BaseDexterity = new BaseStats(40);
        BaseConstitution = new BaseStats(20);
        BaseAgility = new BaseStats(20);
        BaseIntelligence = new BaseStats(20);
        BaseCharisma = new BaseStats(20);

        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
        CurrentWeight = 0;

        Speed = 2;
        AngularSpeed = 999;
        Acceleration = 8;

        Level = 0;
        MaxExperience = 10 + (Level * 10) + (Level + 1) ^ 3;
        CurrentExperience = 0;

        Fame = 0;
        Honor = 0;
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
