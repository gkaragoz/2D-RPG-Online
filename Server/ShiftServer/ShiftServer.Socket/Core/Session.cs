using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    public class Session
    {
        private string sid { get; set; } = null;

        public void SetSid(ShiftServerData data)
        {
            if (data.ClData == null)
                return;

            string guid = Guid.NewGuid().ToString();
            string machineName = data.ClData.MachineName;
            string machineID = data.ClData.MachineId;
            string salt = guid + machineName + machineID;
            sid = Crypto.MD5Hash(salt);
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
