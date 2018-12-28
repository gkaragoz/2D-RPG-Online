using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public void Connect()
        {
            try
            {   
                // create and connect the client
                client = new Telepathy.Client();
                client.Connect("localhost", 1337);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {

            }
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
        public void SendMessage()
        {
            if (client == null)
            {
                return;
            }


            // send a message to server
            client.Send(new byte[] { 0xFF });
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
