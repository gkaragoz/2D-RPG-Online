using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Db
{
    public class AccountFriendRequests
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("requester_id")]
        public string Requester { get; set; }

        [BsonElement("reciever_id")]
        public string Reciever { get; set; }

        [BsonElement("stage")]
        public string Stage { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

    }
}
