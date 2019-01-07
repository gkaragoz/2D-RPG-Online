using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public int HttpCode { get; set; }
        public string Session { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorType { get; set; }
    }

    public enum AuthError
    {
        USER_NOT_FOUND = 0,
        USERNAME_ALREADY_EXIST = 1,
        EMAIL_INVALID = 2,
        USERNAME_INVALID = 3,
        CREDS_INVALID = 4,
        USER_NOT_CONFIRMED = 5
    }
}
