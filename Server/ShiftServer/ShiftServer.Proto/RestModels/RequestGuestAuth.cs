using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class RequestGuestAuth
    {
        public string guest_id;
    }
}
