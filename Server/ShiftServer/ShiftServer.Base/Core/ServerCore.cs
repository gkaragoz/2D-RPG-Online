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
                    TcpClient client = ServerProvider.instance.server.GetClient(msg.connectionId);
                    ShiftClient shift = null;

                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            try
                            {
                                shift = new ShiftClient
                                {
                                    ConnectonID = msg.connectionId,
                                    Client = client,
                                    IsJoinedToWorld = true,
                                    IsJoinedToTeam = false,
                                    IsJoinedToRoom = false,
                                    IsReady = false,

                                    UserSessionID = new Session()
                                };
                                ServerProvider.instance.world.Clients.Add(shift.ConnectonID, shift);
                                if (client != null)
                                {
                                    ServerProvider.log.Info("Connected from: " + client.Client.RemoteEndPoint.ToString());
                                    break;

                                }
                                break;
                            }
                            catch (Exception err)
                            {
                                ServerProvider.log.Error("Error on connection ", err);
                                continue;
                            }


                        case Telepathy.EventType.Data:
                            ServerProvider.instance.world.Clients.TryGetValue(msg.connectionId, out shift);
                            int registeredSocketId = 0;

                            ServerProvider.instance.events.HandleMessage(msg.data, shift);

                            break;
                        case Telepathy.EventType.Disconnected:
                            try
                            {
                                ShiftClient dcedClient = null;
                                ServerProvider.instance.world.Clients.TryGetValue(msg.connectionId, out dcedClient);

                                dcedClient.Dispose();

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
                Thread.Sleep(1000 / 60);
            }

        }
    }
}
