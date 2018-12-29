using System;

namespace ShiftServer.Socket.Core
{
    public class ServerProvider
    {
        private Telepathy.Server server = null;
        private int clientCount = 0;
        public ServerProvider() { }

        public void Listen()
        {
            server = new Telepathy.Server();
            server.Start(1337);
            
        }

        public void Update()
        {
            // grab all new messages. do this in your Update loop.
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Console.WriteLine(msg.connectionId + " Connected");
                        clientCount++;
                        break;
                    case Telepathy.EventType.Data:
                        //TODO burasi async olmak zorunda
                        
                        MsgManager.HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        clientCount--;
                        Console.WriteLine(msg.connectionId + " Disconnected");
                        break;
                }
            }
        }


        public void SendData()
        {
            // create and start the server
            // send a message to client with connectionId = 0 (first one)
            server.Send(0, new byte[] { 0x42, 0x13, 0x37 });
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
