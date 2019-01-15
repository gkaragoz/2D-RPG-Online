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
    public class CharacterService
    {
        private readonly IMongoCollection<Character> _characters;

        public CharacterService(string connectionString, string AccountDbName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(AccountDbName);
            _characters = database.GetCollection<Character>("character");
        }

        public List<Character> Get()
        {
            return _characters.Find(ch => true).ToList();
        }


        public Character FindByCharName(string name)
        {
            return _characters.Find<Character>(ch => ch.Name == name).FirstOrDefault();
        }

        public void Remove(Character chIn)
        {
            _characters.DeleteOne(ch => ch.Name == chIn.Name);
        }

        public void Remove(ObjectId id)
        {
            _characters.DeleteOne(acc => acc.ID == id);
        }
    }
}
