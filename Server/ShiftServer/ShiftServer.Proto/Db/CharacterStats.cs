using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class CharacterStats
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("character_id")]
        public string CharID { get; set; }

        [BsonElement("health")]
        public int Health { get; set; }

        [BsonElement("mana")]
        public int Mana { get; set; }

        [BsonElement("movement_speed")]
        public double MovementSpeed { get; set; }

        [BsonElement("attack_speed")]
        public double AttackSpeed { get; set; }

        [BsonElement("attack_range")]
        public double AttackRange { get; set; }

        [BsonElement("attack_damage")]
        public int AttackDamage { get; set; }
    }
}
