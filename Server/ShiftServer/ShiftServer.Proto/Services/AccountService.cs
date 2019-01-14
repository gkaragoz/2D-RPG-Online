using MongoDB.Driver;
using ShiftServer.Proto.Db;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using MongoDB.Bson;

namespace ShiftServer.Proto.Services
{
    public class AccountService
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountService(string connectionString, string AccountDbName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(AccountDbName);
            _accounts = database.GetCollection<Account>("Accounts");
        }

        public List<Account> Get()
        {
            return _accounts.Find(account => true).ToList();
        }

        public Account Get(string id)
        {
            var docId = new ObjectId(id);

            return _accounts.Find<Account>(account => account.ID == docId).FirstOrDefault();
        }

        public Account Create(Account acc)
        {
            _accounts.InsertOne(acc);
            return acc;
        }

        public void Update(string id, Account accIn)
        {
            var docId = new ObjectId(id);

            _accounts.ReplaceOne(acc => acc.ID == docId, accIn);
        }

        public void Remove(Account accIn)
        {
            _accounts.DeleteOne(acc => acc.ID == accIn.ID);
        }

        public void Remove(ObjectId id)
        {
            _accounts.DeleteOne(acc => acc.ID == id);
        }
    }

}

