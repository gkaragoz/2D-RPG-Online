using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
 
    public class AuthResponse
    {
        public string AccessToken;
        public string Code;
        public string ErrorMessage;
        public int ExpireIn;
        public string IdToken;
        public string RefreshToken;
        public string Session;
        public bool Success;
    }
}
