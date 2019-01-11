using System;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class AuthResponse
    {
        public string session;
        public bool success;
    }
}
