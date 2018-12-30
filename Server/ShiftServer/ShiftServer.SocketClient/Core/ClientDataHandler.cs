using ShiftServer.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class ClientDataHandler
    {
        public List<ClientEventCallback> clientEvents = null;
        public List<PlayerEventCallback> playerEvents = null;

        public ClientDataHandler()
        {
            clientEvents = new List<ClientEventCallback>();
            playerEvents = new List<PlayerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);

            switch (data.Basevtid)
            {
                case MSBaseEventId.MsPlayerEvent:
                    ClientEventInvoker.Fire(playerEvents, data);
                    break;
                case MSBaseEventId.MsServerEvent:
                    ClientEventInvoker.Fire(clientEvents, data);
                    break;
                default:

                    break;

            }
        }
    }
}
