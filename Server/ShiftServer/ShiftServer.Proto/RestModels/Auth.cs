using System;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class Auth
    {
        public string session_id;
        public string user_id;
        public bool success;
        public bool email_verified;
    }
}
