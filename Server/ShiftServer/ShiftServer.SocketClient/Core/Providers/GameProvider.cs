using System;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Google.Protobuf;
using ShiftServer.Client;
using ShiftServer.Proto.Helper;

namespace ShiftServer.Client.Core
{
    /// <summary>
    /// Shift Server Core Methods
    /// </summary>
    public class GameProvider
    {
        public Mirror.Transport.Tcp.Client client = null;
        public Thread listenerThread = null;
        public ClientDataHandler dataHandler = null;
        string _address = null;
        int _port;
        public GameProvider()
        {
            dataHandler = new ClientDataHandler();
        }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Connect(string address, int port)
        {
            try
            {
                if (client == null)
                {
                    _port = port;
                    _address = address;

                    // create and connect the client
                    client = new Mirror.Transport.Tcp.Client();

                    //this.SetFixedUpdateInterval();
                    client.Connect(address, port);
                }
                else
                {
                    throw new Exception("Already connected");
                }

            }
            catch (Exception)
            {
                this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionFailed, ShiftServerError.NoRespondServer);
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
            return client.IsConnected;
        }
        public bool IsClientConnected()
        {
            if (this.client == null)
                return false;

            return client.IsConnected;
        }
        public void Disconnect()
        {
            try
            {
                // disconnect from the server when we are done
                client.Disconnect();
                client = null;
            }
            catch (Exception err)
            {

                this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionLost, ShiftServerError.NoRespondServer);
                return;
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


        public void SetupTasks()
        {
            try
            {
              
                client.ReceivedData += Client_ReceivedData;
                client.Connected += Client_Connected;
                client.Disconnected += Client_Disconnected;
                client.ReceivedError += Client_ReceivedError;               
            
            }
            catch (SocketException socketException)
            {
                this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionLost, ShiftServerError.NoRespondServer);
                Console.WriteLine("Socket exception: " + socketException);
            }
        }

        private void Client_ReceivedError(Exception obj)
        {
            Console.WriteLine("Socket exception: " + obj);
        }

        private void Client_Disconnected()
        {
            this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionLost, ShiftServerError.NoRespondServer);
            client.Disconnect();
        }

        private void Client_Connected()
        {
            this.dataHandler.HandleServerSuccess(MSServerEvent.Connection);
        }

        private void Client_ReceivedData(byte[] obj)
        {
            this.dataHandler.HandleMessage(obj);
        }
    }
}
