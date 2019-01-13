using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    public class CharAddRequest
    {
        public string session_id;
        public string char_name;
        public int char_class;
    }
}
