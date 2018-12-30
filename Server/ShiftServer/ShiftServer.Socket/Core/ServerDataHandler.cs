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
        public List<ServerEventCallback> serverEvents = null;
        public List<PlayerEventCallback> playerEvents = null;

        public ServerDataHandler()
        {
            serverEvents = new List<ServerEventCallback>();
            playerEvents = new List<PlayerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb, ShiftClient client)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);

            switch (data.Basevtid)
            {
                case MSBaseEventId.MsPlayerEvent:
                    ServerEventInvoker.Fire(playerEvents, data, client);
                    break;
                case MSBaseEventId.MsServerEvent:
                    ServerEventInvoker.Fire(serverEvents, data, client);
                    break;
                default:
                 
                    break;

            }
        }
    }
}
