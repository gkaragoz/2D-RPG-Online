using Google.Protobuf.WellKnownTypes;
using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IRoom
    {
        string Name { get; set; }
        string Id { get; set; }
        int CreatedUserId { get; set; }
        int MaxConnId { get; set; }
        int ServerLeaderId { get; set; }
        int DisposeInMilliseconds { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdateDate { get; set; }
        bool IsPrivate { get; set; }
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        int MaxUser { get; set; }
        void OnPlayerJoin(ShiftServerData data, ShiftClient client);
        void OnPlayerCreate(ShiftServerData data, ShiftClient client);
        void OnObjectDestroy(IGameObject gameObject);
        void OnObjectCreate(IGameObject gameObject);
        void OnObjectMove(ShiftServerData data, ShiftClient client);
        void OnObjectAttack(ShiftServerData data, ShiftClient client);
        void OnObjectUse(ShiftServerData data, ShiftClient client);
        void OnGameStart(ShiftServerData data, ShiftClient client);
        void BroadcastToRoom(ShiftClient currentClient, MSServerEvent state);
        void OnRoomUpdate();
        void SendRoomState();
    }
}
