using Google.Protobuf;
using ShiftServer.Proto.Helper;
using ShiftServer.Server.Auth;
using ShiftServer.Server.Helper;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace ShiftServer.Server.Core
{
    public class ServerProvider
    {
        private static Telepathy.Server server = null;
        private static IWorld world = null;
        public ServerDataHandler dataHandler = null;
        public Thread listenerThread = null;

        public ServerProvider(IWorld createdWorld) {
            world = createdWorld;
            dataHandler = new ServerDataHandler();
        }

        public void Listen(int tickrate, int port)
        {
            server = new Telepathy.Server();
            server.NoDelay = true;
            server.SendTimeout = 0;
            server.Start(port);

            int timerInterval = TickrateUtil.Set(15);

            SetInterval(timerInterval, UpdateWorld);

            listenerThread = new Thread(GetMessages);
            listenerThread.IsBackground = true;
            listenerThread.Name = "ShiftServer Listener";
            listenerThread.Start();

        }

        internal void AddServerEventListener(MSServerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            this.dataHandler.serverEvents.Add(new ServerEventCallback
            {
                CallbackFunc = listener,
                EventId = eventType
            });
        }
        internal void AddServerEventListener(MSPlayerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            this.dataHandler.playerEvents.Add(new PlayerEventCallback
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
            world.SendWorldState();
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
            while (server.Active)
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
                            data.Basevtid = MSBaseEventId.MsServerEvent;
                            data.Svevtid = MSServerEvent.MsConnectOk;

                            SendDataByConnId(msg.connectionId, data);
                            world.Clients.Add(msg.connectionId, new ShiftClient
                            {
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

                            world.Clients.Remove(msg.connectionId);
                            Console.WriteLine(msg.connectionId + " Disconnected");
                            break;
                        default:
                            break;
                    }

                }
            }
           
        }

        public void SendDataByConnId(int connId, ShiftServerData data)
        {

            byte[] bb = data.ToByteArray();
            server.Send(connId, bb);
        }

        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(int connId, MSServerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.MsServerEvent;
            data.Svevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                server.Send(connId, bb);
        }


        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(int connId, MSPlayerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.MsPlayerEvent;
            data.Plevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                server.Send(connId, bb);
        }


        public void OnPing(ShiftServerData data, ShiftClient shift)
        {
            SendMessage(shift.connectionId, MSServerEvent.MsPingRequest, data);
        }

        public int ClientCount()
        {
            return world.Clients.Count;
        }
        public void Stop()
        {
            // stop the server when you don't need it anymore
            listenerThread.Abort();
            server.Stop();
        }
    }
}
