using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class Character
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("account_id")]
        public string AccountID { get; set; }

        [BsonElement("character_id")]
        public string CharacterID { get; set; }

        [BsonElement("account_email")]
        public string AccountEmail { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("class_index")]
        public int ClassIndex { get; set; }

        [BsonElement("stat_points")]
        public int StatPoints { get; set; }

        [BsonElement("level")]
        public int Level { get; set; }

        [BsonElement("exp")]
        public int Exp { get; set; }

        [BsonElement("stats")]
        public CharacterStats Stats { get; set; }

        [BsonElement("attributes")]
        public CharacterAttributes Attributes { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }

    }
}
