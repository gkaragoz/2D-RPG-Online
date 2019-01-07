using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public Error Error { get; set; }
        public int ExpireIn { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string Session { get; set; }
        public bool Success { get; set; }
    }
}
