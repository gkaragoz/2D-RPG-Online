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
        /// Add event handler function with giving message enum
        /// </summary>
        /// <param name="eventType">Shift Server event list</param>
        /// <param name="listener">Callback function which fired when event triggered</param>
        public void AddEventListener(ShiftServerMsgID eventType, Action<ShiftServerMsg> listener)
        {
            gameProvider.messageManager.events.Add(new EventCallback {
                CallbackFunc = listener,
                EventId = eventType
            });
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
        public void SendMessage(ShiftServerMsg data)
        {
            byte[] bb = gameProvider.CraftData(data);

            if (bb.Length > 0)
                gameProvider.SendMessage(bb);
        }



    }
}
