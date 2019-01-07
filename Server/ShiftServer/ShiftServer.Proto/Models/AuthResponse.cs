using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
 
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public int ExpireIn { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string Session { get; set; }
        public bool Success { get; set; }
    }
}
