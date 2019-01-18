using ShiftServer.Proto.Services;

namespace ShiftServer.Base.Core
{
    public class DBContext
    {
        public static DBContext ctx { get; set; }
        public AccountService Accounts { get; set; }
        public SessionService Sessions { get; set; }
        public CharacterService Characters { get; set; }
        public DBContext()
        {
            ctx = this;
            string dbName = System.Configuration.ConfigurationManager.AppSettings.Get("MongoDBName");
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["ManaMongoSessionServices"].ToString();

            Accounts = new AccountService(conStr, dbName);
            Sessions = new SessionService(conStr, dbName);
            Characters = new CharacterService(conStr, dbName);

            
        }
    }
}
