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
        public Telepathy.Client client = null;
        public Thread listenerThread = null;
        public ClientDataHandler dataHandler = null;
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
                // create and connect the client
                client = new Telepathy.Client();               
                //this.SetFixedUpdateInterval();
                client.Connect(address, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
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
       
        public void SetFixedUpdateInterval()
        {
            //this timer interval simulate the fixed update in unity. must control on server every time
            //int timerInterval = TickrateUtil.Set(15);

            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(FixedUpdate);
            //// Set the Interval to 1 millisecond.  Note: Time is set in Milliseconds
            //aTimer.Interval = timerInterval;
            //aTimer.Enabled = true;

        }

        public void FixedUpdate()
        {
            try
            {
                if (client.Connected)
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
                                this.dataHandler.HandleMessage(msg.data);
                                break;
                            case Telepathy.EventType.Disconnected:
                                Console.WriteLine("Disconnected");
                                client.Disconnect();
                                break;
                        }
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
