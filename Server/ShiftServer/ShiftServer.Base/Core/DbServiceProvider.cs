using ShiftServer.Proto.Services;

namespace ShiftServer.Base.Core
{
    public class DBServiceProvider
    {
        public AccountService Accounts { get; set; }
        public SessionService Sessions { get; set; }
        public CharacterService Characters { get; set; }
        public DBServiceProvider()
        {
            string dbName = System.Configuration.ConfigurationManager.AppSettings.Get("MongoDBName");
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["ManaMongoSessionServices"].ToString();

            Accounts = new AccountService(conStr, dbName);
            Sessions = new SessionService(conStr, dbName);
            Characters = new CharacterService(conStr, dbName);

            
        }
    }
}
