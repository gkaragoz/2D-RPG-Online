
using System;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class CharacterModel
    {
        public string account_id;
        public string account_email;
        public string name;
        public CharacterStats stat;
        public CharacterAttributes attribute;
        public int class_index;
        public int level;
        public int exp;
        public int stat_points;
        public DateTime created_at;
    }

    [Serializable]
    public class CharacterStats
    {

        public int health;
        public int mana;
        public float attack_speed;
        public float movement_speed;
    }

    [Serializable]
    public class CharacterAttributes
    {
        public int strength;
        public int intelligence;
        public int dexterity;
    }

}
