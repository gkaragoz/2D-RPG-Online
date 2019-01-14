using ShiftServer.Base.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class Session
    {
        private string sid { get; set; } = null;

        public void SetSid(ShiftServerData data)
        {
            sid = data.SessionID;
        }

        public string GetSid()
        {
            return sid;
        }
        public bool IsAuthenticated()
        {
            //TODO: AUTH
            return true;
        }
    }
}
