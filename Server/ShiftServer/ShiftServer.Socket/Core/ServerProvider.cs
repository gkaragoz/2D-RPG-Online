using Google.Protobuf;
using System;

namespace ShiftServer.Server.Core
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
                        
                        ShiftServerMsg data = new ShiftServerMsg();
                        data.MsgId = ShiftServerMsgID.ShiftServerConnectOk;
                        var client = server.GetClient(msg.connectionId);
                        SendData(msg.connectionId, data);

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


        public void SendData(int connId, ShiftServerMsg data)
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
