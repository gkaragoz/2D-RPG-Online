using Google.Protobuf;
using ShiftServer.Proto.Handlers;
using ShiftServer.Proto.Models;
using ShiftServer.Server.Auth;
using ShiftServer.Server.Helper;
using System;
using System.Net.Sockets;
using System.Timers;

namespace ShiftServer.Server.Core
{
    public class ServerProvider
    {
        private static Telepathy.Server server = null;
        private static IWorld world = null;
        public ServerDataHandler dataHandler = null;

        public ServerProvider(IWorld createdWorld) {
            world = createdWorld;
        }

        public void Listen(int tickrate)
        {
            server = new Telepathy.Server();
            server.Start(1337);
            int timerInterval = TickrateUtil.Set(15);
            SetInterval(timerInterval, GetPlayerPackets);
            SetInterval(timerInterval, UpdateWorld);

        }

        internal void AddServerEventListener(ServerEventId eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            this.dataHandler.events.Add(new ServerEventCallback
            {
                CallbackFunc = listener,
                EventId = eventType
            });
        }

        
        private void GetPlayerPackets(object source, ElapsedEventArgs e)
        {
            this.GetMessages();
        }
        private void UpdateWorld(object source, ElapsedEventArgs e)
        {
            world.OnWorldUpdate();
        }

        private static void SetInterval(int timerInterval, Action<object, ElapsedEventArgs> job)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(job);
            // Set the Interval to 1 millisecond.  Note: Time is set in Milliseconds
            aTimer.Interval = timerInterval;
            aTimer.Enabled = true;

        }

        public void GetMessages()
        {
            // grab all new messages. do this in your Update loop.
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                TcpClient client = server.GetClient(msg.connectionId);
                ShiftClient shift = null;

                switch (msg.eventType)
                {             
                    case Telepathy.EventType.Connected:

                        Console.WriteLine(msg.connectionId + " Connected");

                        ShiftServerData data = new ShiftServerData();
                        data.Eid = ServerEventId.SConnectOk;
                        SendDataByConnId(msg.connectionId, data);
                        world.Clients.Add(msg.connectionId, new ShiftClient {
                            connectionId = msg.connectionId,
                            Client = client,
                            UserSession = new Session()
                        });
                        break;
                    case Telepathy.EventType.Data:
                        world.Clients.TryGetValue(msg.connectionId, out shift);

                        if (client.Connected)
                            dataHandler.HandleMessage(msg.data, shift);
                        break;
                    case Telepathy.EventType.Disconnected:
                        if (client.Connected)
                        {
                            client.Close();
                            world.Clients.Remove(msg.connectionId);
                        }
                        Console.WriteLine(msg.connectionId + " Disconnected");
                        break;
                    default:
                        break;
                }

            }
        }

        public void SendDataByConnId(int connId, ShiftServerData data)
        {

            byte[] bb = data.ToByteArray();
            server.Send(connId, bb);
        }
        public int ClientCount()
        {
            return world.Clients.Count;
        }
        public void Stop()
        {
            // stop the server when you don't need it anymore
            server.Stop();
        }
    }
}
