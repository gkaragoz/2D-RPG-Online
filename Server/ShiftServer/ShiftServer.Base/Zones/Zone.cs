

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

        public SafeDictionary<string, Thread> _roomThreads = new SafeDictionary<string, Thread>();
        public int ObjectCounter = 0;
        public Zone()
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            GameObjects = new SafeDictionary<int, IGameObject>();

            Rooms = new SafeDictionary<string, IRoom>();
            _roomThreads = new SafeDictionary<string, Thread>();
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

            Thread gameRoom = new Thread(room.OnGameStart)
            {
                IsBackground = true,
                Name = "ShiftServer Room Starts " + room.Name
            };

            //starting game thread
            gameRoom.Start();

            _roomThreads.Add(room.ID, gameRoom);

        }
        public void RemoveRoom(IRoom room)
        {
          

        }

    }
}
