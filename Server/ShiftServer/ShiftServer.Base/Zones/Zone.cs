

using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using System.Threading;
using Telepathy;

namespace ShiftServer.Base.Zones
{
    public class Zone : IZone
    {
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, IRoom> Rooms { get; set; }
        public SafeDictionary<string, Thread> roomThreads { get; set; }

        public int ObjectCounter = 0;
        public Zone()
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            roomThreads = new SafeDictionary<string, Thread>();
            Rooms = new SafeDictionary<string, IRoom>();
        }
      
        public void ClientDispose(ShiftClient client)
        {
            this.Clients.Remove(client.ConnectionID);
        }
        public void ClientJoin(ShiftClient client)
        {
            this.Clients.Add(client.ConnectionID, client);
            client.IsJoinedToWorld = true;
        }
        public void AddRoom(IRoom room)
        {
            room.IsStopTriggered = false;

            this.Rooms.Add(room.ID, room);

            //Thread gameRoom = new Thread(room.OnGameStart)
            //{
            //    IsBackground = true,
            //    Name = "ShiftServer Room Starts " + room.Name
            //};

            ////starting game thread
            //gameRoom.Start();

            ServerProvider.log.Info(room.Name + "> ROOM START");

        }
        public void RemoveRoom(IRoom room)
        {
          

        }

    }
}
