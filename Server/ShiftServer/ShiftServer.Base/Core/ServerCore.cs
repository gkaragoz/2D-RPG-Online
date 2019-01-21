using ShiftServer.Base.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        long messagesReceived = 0;
        long dataReceived = 0;
        public static ServerCore instance = null;
        public ServerCore()
        {
            instance = this;
        }
        public void GetMessages()
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();
            ServerProvider.instance.server.ReceivedData += Server_ReceivedDataAsync;
            ServerProvider.instance.server.Connected += Server_Connected;
            ServerProvider.instance.server.Disconnected += Server_Disconnected;
            ServerProvider.instance.server.ReceivedError += Server_ReceivedError;
        }

        public void Server_ReceivedError(int connId, Exception arg2)
        {
            ServerProvider.log.Error($"ClientNO: {connId} Exception " + arg2.Message);
        }

        public void Server_Disconnected(int connId)
        {
            try
            {
                ShiftClient client = null;
                ServerProvider.instance.world.Clients.TryGetValue(connId, out client);

                if (client != null)
                    client.Dispose();

                ServerProvider.log.Info($"ClientNO: {connId} Disconnected");
            }
            catch (Exception err)
            {
                ServerProvider.log.Error($"ClientNO: {connId} Exception throwed when trying to disconnect", err);
            }
        }

        public void Server_Connected(int connId)
        {
            try
            {
                AddNewConnection(connId);
            }
            catch (Exception err)
            {
                ServerProvider.log.Error("Error on connection ", err);
            }
        }

        public void Server_ReceivedDataAsync(int connId, byte[] data)
        {

            ShiftClient shift = null;
            ServerProvider.instance.world.Clients.TryGetValue(connId, out shift);
            ServerProvider.instance.events.HandleMessage(data, shift);
        }

        private void AddNewConnection(int connId)
        {
            TcpClient tcp = ServerProvider.instance.server.GetClient(connId);

            ShiftClient client = new ShiftClient
            {
                ConnectionID = connId,
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
