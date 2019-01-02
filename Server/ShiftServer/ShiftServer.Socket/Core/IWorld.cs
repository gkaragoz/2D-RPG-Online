using ShiftServer.Server.Auth;
using ShiftServer.Server.Factory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IWorld
    {
        Vector3 Scale { get; set; }
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        SafeDictionary<string, IRoom> Rooms { get; set; }
        SafeDictionary<string, Thread> RoomGameThreadList { get; set; }
        void OnAccountLogin(ShiftServerData data, ShiftClient client);
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
