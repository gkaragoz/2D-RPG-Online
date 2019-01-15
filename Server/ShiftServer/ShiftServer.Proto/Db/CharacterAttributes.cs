using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class CharacterAttributes
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("character_id")]
        public string CharID { get; set; }

        [BsonElement("strength")]
        public int Strength { get; set; }

        [BsonElement("intelligence")]
        public int Intelligence { get; set; }

        [BsonElement("dexterity")]
        public int Dexterity { get; set; }
    }
}
