﻿using ShiftServer.Base.Core;
using ShiftServer.Base.Helper;
using ShiftServer.Base.Rooms;
using ShiftServer.Base.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Chat
{
    class ChatServer
    {

        private static ChatProvider _chatProvider = null;

        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {


            ChatServer zone = new ChatServer();
           

           // _chatProvider.AddServerEventListener(MSServerEvent.Login, zone.);

            _chatProvider.Listen(tickrate: 5, port: 2001);

            //ConsoleUI.Run(_chatProvider);
            //Run Server Simulation
        }

    }
}