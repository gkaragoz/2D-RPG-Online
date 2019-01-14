using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class RequestCharSelect
    {
        public string session_id;
        public string char_name;
    }
}
