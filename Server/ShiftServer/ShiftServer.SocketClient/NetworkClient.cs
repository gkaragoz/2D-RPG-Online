using Google.Protobuf;
using ShiftServer.Client.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ShiftServer.Client
{
    /// <summary>
    /// The main <c>network client</c> class.
    /// Contains all methods for performing game server functions.
    /// </summary>
    public sealed class NetworkClient
    {
        private static NetworkClient networkClient = null;
        private static GameProvider gameProvider = null;

        /// <summary>
        /// Constructor method of game client object
        /// </summary>
        public NetworkClient()
        {
            networkClient = this;
            gameProvider = new GameProvider();
        }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Connect(string address, int port)
        {
            gameProvider.Connect(address, port);
        }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public bool IsConnected()
        {
            return gameProvider.IsConnected();
        }
        /// <summary>
        /// Fixed Update
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Listen()
        {
            gameProvider.FixedUpdate();
        }

        /// <summary>
        /// Add event handler function with giving message enum
        /// </summary>
        /// <param name="eventType">Shift Server event list</param>
        /// <param name="listener">Callback function which fired when event triggered</param>
        public void AddEventListener(object eventType, Action<ShiftServerData> listener)
        {
            if (eventType.GetType() == typeof(MSServerEvent))
            {
                gameProvider.dataHandler.clientEvents.Add(new ClientEventCallback
                {
                    CallbackFunc = listener,
                    EventId = (MSServerEvent)eventType
                });
            }
            else if (eventType.GetType() == typeof(MSPlayerEvent))
            {
                gameProvider.dataHandler.playerEvents.Add(new PlayerEventCallback
                {
                    CallbackFunc = listener,
                    EventId = (MSPlayerEvent)eventType
                });
            }
          
        }
        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            gameProvider.Disconnect();
        }

      

        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(MSServerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.MsServerEvent;
            data.Svevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                gameProvider.SendMessage(bb);
        }


        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(MSPlayerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.MsPlayerEvent;
            data.Plevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                gameProvider.SendMessage(bb);
        }


    }
}
