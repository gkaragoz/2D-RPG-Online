﻿using Google.Protobuf;
using ShiftServer.Client.Core;
using ShiftServer.Client.Data.Entities;
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
    public sealed class ManaShiftServer
    {
        private static ManaShiftServer _mss = null;
        private static GameProvider gameProvider = null;
        public bool IsConnected { get => gameProvider.client.Connected; }

        public bool IsConnecting{ get => gameProvider.client.Connecting; }

        /// <summary>
        /// Constructor method of game client object
        /// </summary>
        public ManaShiftServer()
        {
            _mss = this;
            gameProvider = new GameProvider();
        }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Connect(ConfigData cfg)
        {
            gameProvider.Connect(cfg.Host, cfg.Port);
        }   
        
        /// <summary>
        /// Get Room List
        /// </summary>
        public List<Room> GetRoomList()
        {
            return gameProvider.dataHandler.roomProvider.RoomList;
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
        public void SendMessage(MSServerEvent evt, ShiftServerData data = null)
        {
            if (data == null)
                data = new ShiftServerData();

            data.Basevtid = MSBaseEventId.ServerEvent;
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
            if (data == null)
                throw new Exception();

            data.Basevtid = MSBaseEventId.PlayerEvent;
            data.Plevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                gameProvider.SendMessage(bb);
        }


    }
}