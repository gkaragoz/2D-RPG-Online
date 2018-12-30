using ShiftServer.Server.Core;
using ShiftServer.Server.Factory.Movement;
using ShiftServer.Server.Helper;
using ShiftServer.Server.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShiftServer.Server
{
    class Program
    {
        private static ServerProvider serverProvider = null;
        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {


            RPGWorld world = new RPGWorld();

            serverProvider = new ServerProvider(world);
            serverProvider.AddServerEventListener(MSServerEvent.MsPingRequest, serverProvider.OnPing);

            serverProvider.AddServerEventListener(MSServerEvent.MsJoinRequest, world.OnPlayerJoin);
            serverProvider.AddServerEventListener(MSPlayerEvent.MsOuse, world.OnObjectUse);
            serverProvider.Listen(tickrate : 15);
            ConsoleUI.Run(serverProvider);
            //Run Server Simulation
        }
     
      


    }
}
