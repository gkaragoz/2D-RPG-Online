using ShiftServer.Base.Core;
using ShiftServer.Base.Helper;
using ShiftServer.Base.Rooms;
using ShiftServer.Base.Worlds;


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


            GameZone zone = new GameZone();
            _serverProvider = new ServerProvider(zone);
            _roomProvider = new RoomProvider();
            _groupProvider = new GroupProvider(_roomProvider);

            //default
            Battleground bgRoom = new Battleground(2, 1500);
            bgRoom.Name = "MANASHIFT DEFAULT";
            bgRoom.MaxUser = 3000;
            bgRoom.IsPersistence = true;
            bgRoom.ID = "banaismailde";
            _roomProvider.CreateRoom(bgRoom);

            _serverProvider.AddServerEventListener(MSServerEvent.PingRequest, _serverProvider.OnPing);
            _serverProvider.AddServerEventListener(MSServerEvent.AccountJoin, _serverProvider.OnAccountJoin);

            _serverProvider.AddServerEventListener(MSServerEvent.RoomCreate, _roomProvider.OnRoomCreate);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomDelete, _roomProvider.OnRoomDelete);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomJoin, _roomProvider.OnRoomJoin);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomLeave, _roomProvider.OnRoomLeave);
            _serverProvider.AddServerEventListener(MSServerEvent.RoomChangeLeader, _roomProvider.OnRoomLeaderChange);

            _serverProvider.AddServerEventListener(MSPlayerEvent.Move, _roomProvider.OnObjectMove);

            _serverProvider.Listen(tickrate : 15, port : 2000);

            ConsoleUI.Run(_serverProvider);
            //Run Server Simulation
        } 

    }
}
