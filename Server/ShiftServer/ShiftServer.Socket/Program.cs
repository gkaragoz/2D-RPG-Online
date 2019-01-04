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
        private static GroupProvider _groupProvider = null;
        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {


            Zone zone = new Zone();
            _serverProvider = new ServerProvider(zone);
            _roomProvider = new RoomProvider(_serverProvider);
            _groupProvider = new GroupProvider(_roomProvider);

            Battleground bgRoom = new Battleground(2, 4);
            bgRoom.Name = "MANASHIFT BattleRoyale #1 ( OFFICIAL )";
            bgRoom.MaxUser = 8;
            _roomProvider.CreateRoom(bgRoom);

            _serverProvider.AddServerEventListener(MSServerEvent.PingRequest, _serverProvider.OnPing);

            _serverProvider.AddServerEventListener(MSServerEvent.Login, zone.OnAccountLogin);

            _serverProvider.AddServerEventListener(MSServerEvent.RoomCreate, _roomProvider.OnRoomCreate);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomDelete, _roomProvider.OnRoomDelete);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomJoin, _roomProvider.OnRoomJoin);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomLeave, _roomProvider.OnRoomLeave);
            _serverProvider.AddServerEventListener(MSServerEvent.LobbyRefresh, _roomProvider.OnLobbyRefresh);

            _serverProvider.AddServerEventListener(MSPlayerEvent.Use, zone.OnObjectUse);
            _serverProvider.AddServerEventListener(MSPlayerEvent.CreatePlayer, zone.OnPlayerCreate);

            _serverProvider.Listen(tickrate : 75, port : 2000);

            ConsoleUI.Run(_serverProvider);
            //Run Server Simulation
        } 

    }
}
