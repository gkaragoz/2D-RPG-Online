using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Data.Entities
{
    public class ConfigData
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SessionID { get; set; }

        public ConfigData() {}
    }
}
