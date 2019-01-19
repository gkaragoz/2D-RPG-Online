using ShiftServer.Base.Auth;
using System.Numerics;
using System.Threading;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IZone
    {
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, IRoom> Rooms { get; set; }
        void ClientJoin(ShiftClient client);
        void AddRoom(IRoom room);
        void ClientDispose(ShiftClient client);
    }
}
