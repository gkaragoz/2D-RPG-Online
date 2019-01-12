using System;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class AuthResponse
    {
        public string session_id;
        public string user_id;
        public bool success;
        public bool is_guest;
        public bool email_verified;
    }
}
