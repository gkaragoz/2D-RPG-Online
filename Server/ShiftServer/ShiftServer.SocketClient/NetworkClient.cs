using ShiftServer.SocketClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShiftServer.SocketClient
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
        /// Connect is triggered when login event is fired
        /// </summary>
        public void Connect()
        {
            gameProvider.Connect();
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            gameProvider.Disconnect();
        }

        /// <summary>
        /// Put this method to your FixedUpdate function
        /// </summary>
        public void Receive()
        {
            gameProvider.ListenForData();
        }

        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(ShiftServerMsgID shiftServerMsgID)
        {
            byte[] bb = gameProvider.CraftData(shiftServerMsgID);

            if (bb.Length > 0)
                gameProvider.SendMessage(bb);
        }

       

    }
}
