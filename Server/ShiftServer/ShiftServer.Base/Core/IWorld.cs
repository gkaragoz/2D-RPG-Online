using ShiftServer.Base.Auth;
using System.Numerics;
using System.Threading;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IZone
    {
        Vector3 Scale { get; set; }
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        SafeDictionary<string, IRoom> Rooms { get; set; }
        SafeDictionary<string, Thread> RoomGameThreadList { get; set; }
        void OnPlayerJoin(ShiftServerData data, ShiftClient client);
        void OnPlayerCreate(ShiftServerData data, ShiftClient client);
        void OnObjectDestroy(IGameObject gameObject);
        void OnObjectCreate(IGameObject gameObject);
        void OnObjectMove(ShiftServerData data, ShiftClient client);
        void OnObjectAttack(ShiftServerData data, ShiftClient client);
        void OnObjectUse(ShiftServerData data, ShiftClient client);
        void OnWorldUpdate();
        void SendWorldState();
    }
}
