using ShiftServer.Base.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShiftServer.Base.Core
{
    public class ServerCore
    {
        public static ServerCore instance = null;
        public ServerCore()
        {
            instance = this;
        }
        public void GetMessages()
        {
            while (ServerProvider.instance.server.Active)
            {
                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;
                while (ServerProvider.instance.server.GetNextMessage(out msg))
                {

                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            try
                            {
                                AddNewConnection(msg);
                                break;
                            }
                            catch (Exception err)
                            {
                                ServerProvider.log.Error("Error on connection ", err);
                                continue;
                            }


                        case Telepathy.EventType.Data:

                            ShiftClient shift = null;
                            ServerProvider.instance.world.Clients.TryGetValue(msg.connectionId, out shift);
                            ServerProvider.instance.events.HandleMessage(msg.data, shift);

                            break;
                        case Telepathy.EventType.Disconnected:
                            try
                            {
                                ShiftClient client = null;
                                ServerProvider.instance.world.Clients.TryGetValue(msg.connectionId, out client);

                                if (client != null)
                                    client.Dispose();

                                ServerProvider.log.Info($"ClientNO: {msg.connectionId} Disconnected");
                            }
                            catch (Exception err)
                            {
                                ServerProvider.log.Error($"ClientNO: {msg.connectionId} Exception throwed when trying to disconnect", err);
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void AddNewConnection(Telepathy.Message msg)
        {
            TcpClient tcp = ServerProvider.instance.server.GetClient(msg.connectionId);

            ShiftClient client = new ShiftClient
            {
                ConnectionID = msg.connectionId,
                TCPClient = tcp,
                IsJoinedToWorld = false,
                IsJoinedToTeam = false,
                IsJoinedToRoom = false,
                IsReady = false,
                UserSessionID = "",
            };

            ServerProvider.instance.world.ClientJoin(client);

            if (client != null)
            {
                ServerProvider.log.Info("Connected from: " + tcp.Client.RemoteEndPoint.ToString());
            }

        }
    }
}
