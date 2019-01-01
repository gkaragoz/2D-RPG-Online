using ShiftServer.Server.Core;
using ShiftServer.Server.Factory.Movement;
using ShiftServer.Server.Helper;
using ShiftServer.Server.Worlds;


namespace ShiftServer.Server
{
    class Program
    {
        private static readonly log4net.ILog log
              = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ServerProvider serverProvider = null;
        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {


            RPGWorld world = new RPGWorld();

            serverProvider = new ServerProvider(world);
            serverProvider.AddServerEventListener(MSServerEvent.PingRequest, serverProvider.OnPing);

            serverProvider.AddServerEventListener(MSServerEvent.Login, world.OnAccountLogin);
            serverProvider.AddServerEventListener(MSServerEvent.RoomJoin, world.OnPlayerJoin);

            serverProvider.AddServerEventListener(MSPlayerEvent.Use, world.OnObjectUse);
            serverProvider.AddServerEventListener(MSPlayerEvent.CreatePlayer, world.OnPlayerCreate);

            serverProvider.Listen(tickrate : 15, port : 2000);

            ConsoleUI.Run(serverProvider);
            //Run Server Simulation
        } 

    }
}
