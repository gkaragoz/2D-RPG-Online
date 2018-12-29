using Google.Protobuf;
using ShiftServer.Proto.Handlers;
using ShiftServer.Proto.Models;
using System;
using System.Net.Sockets;

namespace ShiftServer.Server.Core
{
    public class ServerProvider
    {
        private Telepathy.Server server = null;
        public DataHandler dataHandler = null;
        private int clientCount = 0;
        public ServerProvider() { }

        public void Listen()
        {
            server = new Telepathy.Server();
            server.Start(1337);
            
        }

        public void AddEventListener(ServerEventId eventType, Action<ShiftServerData> listener)
        {
            this.dataHandler.events.Add(new EventCallback
            {
                CallbackFunc = listener,
                EventId = eventType
            });
        }
        public void Update()
        {
            // grab all new messages. do this in your Update loop.
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                TcpClient client = server.GetClient(msg.connectionId);
                switch (msg.eventType)
                {             
                    case Telepathy.EventType.Connected:

                        Console.WriteLine(msg.connectionId + " Connected");

                        ShiftServerData data = new ShiftServerData();
                        data.Eid = ServerEventId.SConnectOk;
                        SendData(msg.connectionId, data);

                        clientCount++;
                        break;
                    case Telepathy.EventType.Data:
                        dataHandler.HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        clientCount--;
                        Console.WriteLine(msg.connectionId + " Disconnected");
                        break;
                }
            }
        }


        public void SendData(int connId, ShiftServerData data)
        {

            byte[] bb = data.ToByteArray();
            server.Send(connId, bb);
        }
        public int ClientCount()
        {
            return clientCount;
        }
        public void Stop()
        {
            // stop the server when you don't need it anymore
            server.Stop();
        }
    }
}
