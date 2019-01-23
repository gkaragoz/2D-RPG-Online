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
            _accounts = database.GetCollection<Account>("account");
        }

        public List<Account> Get()
        {
            return _accounts.Find(account => true).ToList();
        }

        public Account GetByUserID(string id)
        {

            return _accounts.Find<Account>(account => account.UserId == id).FirstOrDefault();
        }

        public Account Create(Account acc)
        {
            _accounts.InsertOne(acc);
            return acc;
        }

        public void Update(string userId, Account accIn)
        {

            _accounts.ReplaceOne(acc => acc.UserId == userId, accIn);
        }

        public void Remove(Account accIn)
        {
            _accounts.DeleteOne(acc => acc.UserId == accIn.UserId);
        }
    }

}

