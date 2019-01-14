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
                _port = port;
                _address = address;

                // create and connect the client
                client = new Telepathy.Client();
                //this.SetFixedUpdateInterval();
                client.Connect(address, port);
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
            return client.Connected;
        }
        public bool IsClientConnected()
        {
            if (this.client == null)
                return false;

            return client.Connected;
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
                if (client == null)
                    return;

                if (client.Connected)
                {
                    // grab all new messages. do this in your Update loop.
                    Telepathy.Message msg;
                    while (client.GetNextMessage(out msg))
                    {
                        switch (msg.eventType)
                        {
                            case Telepathy.EventType.Connected:
                                Console.WriteLine("Connected To Socket");
                                //this.dataHandler.HandleServerSuccess(MSServerEvent.Connection);
                                break;
                            case Telepathy.EventType.Data:
                                this.dataHandler.HandleMessage(msg.data);
                                break;
                            case Telepathy.EventType.Disconnected:
                                Console.WriteLine("Disconnected From Socket");
                                this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionLost, ShiftServerError.NoRespondServer);
                                client.Disconnect();
                                break;
                        }
                    }
                }
                else
                {
                    client.Connect(_address, _port);
                    TickrateUtil.SafeDelay(5);
                }
            }
            catch (SocketException socketException)
            {
                this.dataHandler.HandleServerFailure(MSServerEvent.ConnectionLost, ShiftServerError.NoRespondServer);
                Console.WriteLine("Socket exception: " + socketException);
            }
        }



    }
}
