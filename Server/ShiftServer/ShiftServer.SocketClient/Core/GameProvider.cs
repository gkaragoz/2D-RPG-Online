using System;
using System.Net.Sockets;
using Google.Protobuf;

namespace ShiftServer.SocketClient.Core
{
    /// <summary>
    /// Shift Server Core Methods
    /// </summary>
    public class GameProvider
    {
        public Telepathy.Client client = null;
        public GameProvider() { }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Connect(string address, int port)
        {
            try
            {   
                // create and connect the client
                client = new Telepathy.Client();
                client.Connect(address, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {

            }
        }

        public bool IsConnecting()
        {
            return client.Connecting;
        }

        public bool IsConnected()
        {
            return client.Connected;
        }
        public void Disconnect()
        {
            try
            {
                // disconnect from the server when we are done
                client.Disconnect();
            }
            catch (Exception)
            {

                throw;
            }
        }
       
        /// <summary> 	
        /// Send message to server using socket connection. 	
        /// </summary> 	
        public void SendMessage(byte[] bb)
        {
            if (client == null)
            {
                return;
            }
            // send a message to server
            client.Send(bb);
        }

        public byte[] CraftData(ShiftServerMsgID shiftServerMsgID)
        {
            ShiftServerMsg msg = new ShiftServerMsg
            {
                MsgId = shiftServerMsgID
            };

            return msg.ToByteArray();
        }

        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incomming data. 	
        /// </summary>     
        public void ListenForData()
        {
            try
            {

                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;
                while (client.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Console.WriteLine("Connected");
                            break;
                        case Telepathy.EventType.Data:
                            Console.WriteLine("Data: " + BitConverter.ToString(msg.data));
                            break;
                        case Telepathy.EventType.Disconnected:
                            Console.WriteLine("Disconnected");
                            break;
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }
    }
}
