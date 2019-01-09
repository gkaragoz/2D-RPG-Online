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
            _sessions = database.GetCollection<AccountSession>("AccountSessions");
        }

        public List<AccountSession> Get()
        {
            return _sessions.Find(session => true).ToList();
        }

        public AccountSession Get(string id)
        {
            var docId = new ObjectId(id);

            return _sessions.Find<AccountSession>(session => session.ID == docId).FirstOrDefault();
        }

        public AccountSession FindByAccessToken(string accesstoken)
        {
            return _sessions.Find<AccountSession>(session => session.SessionID == accesstoken).FirstOrDefault();
        }

        public AccountSession Create(AccountSession sess)
        {
            _sessions.InsertOne(sess);
            return sess;
        }

        public void Update(string id, AccountSession sessIn)
        {
            var docId = new ObjectId(id);

            _sessions.ReplaceOne(acc => acc.ID == docId, sessIn);
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
