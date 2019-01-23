using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class AccountSession
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("user_id")]
        public string UserID { get; set; }

        [BsonElement("session_id")]
        public string SessionID { get; set; }

        [BsonElement("is_guest")]
        public bool IsGuest { get; set; }

        [BsonElement("expire_in")]
        public int ExpireIn { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }
    }
}
