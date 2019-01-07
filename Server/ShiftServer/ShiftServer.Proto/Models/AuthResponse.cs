using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    public class Error
    {
        public string Code;
        public string Message;
    }

    public class AuthResponse
    {
        public string Success;
        public int HttpCode;
        public string Session;
        public string AccessToken;
        public string RefreshToken;
        public string IdToken;
        public string ExpireIn;
        public Error Error;
    }
}
