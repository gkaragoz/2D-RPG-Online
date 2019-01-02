using ShiftServer.Server.Core;
using ShiftServer.Server.Factory.Movement;
using ShiftServer.Server.Helper;
using ShiftServer.Server.Rooms;
using ShiftServer.Server.Worlds;


namespace ShiftServer.Server
{
    class Program
    {
        private static readonly log4net.ILog log
              = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ServerProvider _serverProvider = null;
        private static RoomProvider _roomProvider = null;
        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {


            RPGWorld world = new RPGWorld();

            _serverProvider = new ServerProvider(world);
            _roomProvider = new RoomProvider(_serverProvider);

            BattlegroundRoom bgRoom = new BattlegroundRoom();

            bgRoom.Name = "MANASHIFT BattleRoyale #1 ( OFFICIAL )";
            bgRoom.MaxUser = 10;
            _roomProvider.CreateRoom(bgRoom);

            _serverProvider.AddServerEventListener(MSServerEvent.PingRequest, _serverProvider.OnPing);

            _serverProvider.AddServerEventListener(MSServerEvent.Login, world.OnAccountLogin);

            //serverProvider.AddServerEventListener(MSServerEvent.RoomJoin, world.OnRom);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomCreate, _roomProvider.OnRoomCreate);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomJoin, _roomProvider.OnRoomJoin);

            _serverProvider.AddServerEventListener(MSPlayerEvent.Use, world.OnObjectUse);
            _serverProvider.AddServerEventListener(MSPlayerEvent.CreatePlayer, world.OnPlayerCreate);

            _serverProvider.Listen(tickrate : 75, port : 2000);

            ConsoleUI.Run(_serverProvider);
            //Run Server Simulation
        } 

    }
}
