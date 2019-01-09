using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class AccountStats
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("account_id")]
        public string AccountId { get; set; }

        [BsonElement("total_match_count")]
        public string TotalMatchCount { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }
    }
}
