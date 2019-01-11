using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class AddCharResponse
    {
        public string error_message;
        public bool success;
        public Character character;
    }
}
