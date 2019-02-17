using UnityEngine;

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
    private PlayerClass _characterClass;

    [SerializeField]
    private int _statsPoints = 0;

    [SerializeField]
    private BaseStats _baseStrength;
    [SerializeField]
    private BaseStats _baseDexterity;
    [SerializeField]
    private BaseStats _baseIntelligence;

    [SerializeField]
    private int _maxHealth = 0;
    [SerializeField]
    private int _currentHealth = 0;
    [SerializeField]
    private float _healthRegen = 0;

    [SerializeField]
    private int _maxMana = 0;
    [SerializeField]
    private int _currentMana = 0;
    [SerializeField]
    private float _manaRegen = 0;

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

    public PlayerClass CharacterClass {
        get {
            return _characterClass;
        } 

        set {
            _characterClass = value;
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

    public int MaxHealth {
        get {
            return _maxHealth;
        }

        set {
            _maxHealth = value;
        }
    }

    public int CurrentHealth {
        get {
            return _currentHealth;
        }

        set {
            _currentHealth = value;
        }
    }

    public float HealthRegen {
        get {
            return _healthRegen;
        }

        set {
            _healthRegen = value;
        }
    }

    public int MaxMana {
        get {
            return _maxMana;
        }

        set {
            _maxMana = value;
        }
    }

    public int CurrentMana {
        get {
            return _currentMana;
        }

        set {
            _currentMana = value;
        }
    }

    public float ManaRegen {
        get {
            return _manaRegen;
        }

        set {
            _manaRegen = value;
        }
    }

    public int AttackDamage {
        get {
            return _attackDamage;
        }

        set {
            _attackDamage = value;
        }
    }

    public float AttackSpeed {
        get {
            return _attackSpeed;
        }

        set {
            _attackSpeed = value;
        }
    }

    public float AttackRange {
        get {
            return _attackRange;
        }

        set {
            _attackRange = value;
        }
    }

    public float MovementSpeed {
        get {
            return _movementSpeed;
        }

        set {
            _movementSpeed = value;
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

}
