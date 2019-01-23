using MongoDB.Bson;
using MongoDB.Driver;
using ShiftServer.Proto.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Services
{
    public class SessionService
    {
        private readonly IMongoCollection<AccountSession> _sessions;

        public SessionService(string connectionString, string AccountDbName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(AccountDbName);
            _sessions = database.GetCollection<AccountSession>("account_session");
        }

        public List<AccountSession> Get()
        {
            return _sessions.Find(session => true).ToList();
        }


        public AccountSession FindBySessionID(string sessionId)
        {
            return _sessions.Find<AccountSession>(session => session.SessionID == sessionId).FirstOrDefault();
        }

        public void Remove(AccountSession sessIn)
        {
            _sessions.DeleteOne(acc => acc.ID == sessIn.ID);
        }

        public void Remove(ObjectId id)
        {
            _sessions.DeleteOne(acc => acc.ID == id);
        }
    }

}
