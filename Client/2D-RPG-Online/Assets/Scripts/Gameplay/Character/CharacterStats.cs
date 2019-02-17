using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private CharacterStats_SO _characterDefinition_Template;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private CharacterStats_SO _char;

    #region Initializations

    private void Awake() {
        if (_characterDefinition_Template != null) {
            _char = Instantiate(_characterDefinition_Template);
        }
    }

    #endregion

    public void Initialize(NetworkIdentifier networkObject) {
        _char.StatsPoints = AccountManager.instance.GetSelectedCharacter().stat_points;
        _char.Name = networkObject.PlayerData.Name;
        _char.CharacterClass = networkObject.PlayerData.PClass;

        _char.BaseStrength = new CharacterStats_SO.BaseStats(networkObject.PlayerData.Strength);
        _char.BaseDexterity = new CharacterStats_SO.BaseStats(networkObject.PlayerData.Dexterity);
        _char.BaseIntelligence = new CharacterStats_SO.BaseStats(networkObject.PlayerData.Intelligence);

        _char.CurrentHealth = networkObject.PlayerData.CurrentHp;
        _char.MaxHealth = networkObject.PlayerData.MaxHp;
        _char.HealthRegen = AccountManager.instance.GetSelectedCharacter().stat.health_regen;
        _char.CurrentMana = networkObject.PlayerData.CurrentMana;
        _char.MaxMana = networkObject.PlayerData.MaxMana;
        _char.ManaRegen = AccountManager.instance.GetSelectedCharacter().stat.mana_regen;

        _char.AttackDamage = networkObject.PlayerData.AttackDamage;
        _char.AttackSpeed = networkObject.PlayerData.AttackSpeed;
        _char.AttackRange = networkObject.PlayerData.AttackRange;
        _char.MovementSpeed = networkObject.PlayerData.MoveSpeed;

        _char.Level = AccountManager.instance.GetSelectedCharacter().level;
        //_char.MaxExperience = ;
        _char.CurrentExperience = AccountManager.instance.GetSelectedCharacter().exp;
    }


    #region Stat Increasers

    public void AddStatsPoints(int statsPointsAmount) {
        _char.StatsPoints += statsPointsAmount;
    }

    public void IncreaseStrength() {
        if (_char.StatsPoints >= _char.BaseStrength.RequiredStatsPoints) {
            SpendStatsPoints(_char.BaseStrength.RequiredStatsPoints);
            _char.BaseStrength.Increase();
        }
    }

    public void IncreaseDexterity() {
        if (_char.StatsPoints >= _char.BaseDexterity.RequiredStatsPoints) {
            SpendStatsPoints(_char.BaseDexterity.RequiredStatsPoints);
            _char.BaseDexterity.Increase();
        }
    }

    public void IncreaseIntelligence() {
        if (_char.StatsPoints >= _char.BaseIntelligence.RequiredStatsPoints) {
            SpendStatsPoints(_char.BaseIntelligence.RequiredStatsPoints);
            _char.BaseIntelligence.Increase();
        }
    }

    public void ApplyHealth(int healthAmount) {
        if ((_char.CurrentHealth + healthAmount) > _char.MaxHealth) {
            _char.CurrentHealth = _char.MaxHealth;
        } else {
            _char.CurrentHealth += healthAmount;
        }
    }

    public void ApplyMana(int manaAmount) {
        if ((_char.CurrentMana + manaAmount) > _char.MaxMana) {
            _char.CurrentMana = _char.MaxMana;
        } else {
            _char.CurrentMana += manaAmount;
        }
    }

    public void AddAttackDamage(int damageAmount) {
        _char.AttackDamage += damageAmount;
    }

    public void AddAttackSpeed(float speedAmount) {
        _char.AttackSpeed += speedAmount;
    }

    public void AddAttackRange(float rangeAmount) {
        _char.AttackRange += rangeAmount;
    }

    public void AddExp(int expAmount) {
        if (_char.CurrentExperience + expAmount >= _char.MaxExperience) {
            int needExpAmount = _char.MaxExperience - _char.CurrentExperience;
            int remainingExpAmount = expAmount - needExpAmount;

            LevelUp();

            if (remainingExpAmount > 0) {
                AddExp(remainingExpAmount);
            } else {
                _char.CurrentExperience += needExpAmount;
            }
        } else {
            _char.CurrentExperience += expAmount;
        }
    }

    #endregion

    #region Stat Reducers

    public void SpendStatsPoints(int amount) {
        _char.StatsPoints -= amount;
    }

    public void TakeDamage(int amount) {
        _char.CurrentHealth -= amount;

        if (_char.CurrentHealth <= 0) {
            _char.CurrentHealth = 0;
        }
    }

    public void TakeMana(int amount) {
        _char.CurrentMana -= amount;

        if (_char.CurrentMana <= 0) {
            _char.CurrentMana = 0;
        }
    }

    public void ReduceAttackDamage(int damageAmount) {
        _char.AttackDamage -= damageAmount;

        if (_char.AttackDamage <= 0) {
            _char.AttackDamage = 0;
        }
    }

    public void ReduceAttackSpeed(float speedAmount) {
        _char.AttackSpeed -= speedAmount;

        if (_char.AttackSpeed <= 0) {
            _char.AttackSpeed = 0;
        }
    }

    public void ReduceAttackRange(float rangeAmount) {
        _char.AttackRange -= rangeAmount;

        if (_char.AttackRange <= 0) {
            _char.AttackRange = 0;
        }
    }

    public void LooseExp(int expAmount) {
        _char.CurrentExperience -= expAmount;

        if (_char.CurrentExperience <= 0) {
            _char.CurrentExperience = 0;
        }
    }

    #endregion

    #region Reporters

    public string GetName() {
        return _char.Name;
    }

    public PlayerClass GetCharacterClass() {
        return _char.CharacterClass;
    }

    public bool IsDeath() {
        return _char.CurrentHealth <= 0;
    }

    public bool CanIncreaseStrength() {
        return GetStatsPoints() >= _char.BaseStrength.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseDexterity() {
        return GetStatsPoints() >= _char.BaseDexterity.RequiredStatsPoints ? true : false;
    }

    public bool CanIncreaseIntelligence() {
        return GetStatsPoints() >= _char.BaseIntelligence.RequiredStatsPoints ? true : false;
    }

    public bool HasStatsPoints() {
        return _char.StatsPoints > 0 ? true : false;
    }

    public int GetStatsPoints() {
        return _char.StatsPoints;
    }

    public int GetBaseStrength() {
        return _char.BaseStrength.Value;
    }

    public int GetBaseDexterity() {
        return _char.BaseDexterity.Value;
    }

    public int GetBaseIntelligence() {
        return _char.BaseIntelligence.Value;
    }

    public int GetMaxHealth() {
        return _char.MaxHealth;
    }

    public int GetCurrentHealth() {
        return _char.CurrentHealth;
    }

    public int GetMaxMana() {
        return _char.MaxMana;
    }

    public int GetCurrentMana() {
        return _char.CurrentMana;
    }

    public int GetAttackDamage() {
        return _char.AttackDamage;
    }

    public float GetAttackSpeed() {
        return _char.AttackSpeed;
    }

    public float GetMovementSpeed() {
        return _char.MovementSpeed;
    }

    public float GetAttackRange() {
        return _char.AttackRange;
    }

    public int GetLevel() {
        return _char.Level;
    }

    public int GetMaxExperience() {
        return _char.MaxExperience;
    }

    public int GetCurrentExperience() {
        return _char.CurrentExperience;
    }

    #endregion

    private void LevelUp() {
        _char.StatsPoints += 12;
        _char.Level++;

        _char.CurrentExperience = 0;
        _char.MaxExperience = 10 + (_char.Level * 10) + (int)Mathf.Pow(_char.Level + 1, 3);

        //LogManager.instance.AddLog("Level up! Your level is: " + Level, Log.Type.Loot);
        //LogManager.instance.AddLog("You have unspend " + StatsPoints + " stats points.", Log.Type.Loot);
    }

}
