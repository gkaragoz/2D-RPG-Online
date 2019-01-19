﻿using ShiftServer.Base.Auth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ShiftServer.Base.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class ServerEventHandler
    {

        public List<ServerEventCallback> serverEvents = null;
        public List<PlayerEventCallback> playerEvents = null;

        public ServerEventHandler()
        {
            serverEvents = new List<ServerEventCallback>();
            playerEvents = new List<PlayerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb, ShiftClient client)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);

            switch (data.Basevtid)
            {
                case MSBaseEventId.PlayerEvent:
                    ServerEventInvoker.Fire(playerEvents, data, client);
                    break;
                case MSBaseEventId.ServerEvent:
                    ServerEventInvoker.Fire(serverEvents, data, client);
                    break;
                default:
                 
                    break;
            }
        }
    }
}