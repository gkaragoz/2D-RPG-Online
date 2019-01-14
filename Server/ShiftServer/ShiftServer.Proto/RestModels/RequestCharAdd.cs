using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.RestModels
{
    public class RequestCharAdd
    {
        public string session_id;
        public string char_name;
        public int class_index;
    }
}
