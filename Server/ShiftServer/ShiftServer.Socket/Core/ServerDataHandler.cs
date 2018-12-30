using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class ServerDataHandler
    {
        public List<ServerEventCallback> events = null;

        public ServerDataHandler()
        {
            events = new List<ServerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb, ShiftClient client)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);


            ServerEventInvoker.Fire(events, data, client);
        }
    }
}
