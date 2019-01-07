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
        public string ErrorMessage { get; set; }
    }
}
