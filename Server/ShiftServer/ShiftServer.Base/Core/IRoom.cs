using ShiftServer.Base.Auth;
using ShiftServer.Proto.Db;
using System;
using System.Collections.Generic;
using System.Timers;
using Telepathy;

namespace ShiftServer.Base.Core
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
        bool IsStopTriggered { get; set; }
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<string, IGroup> Teams { get; set; }
        List<string> TeamIdList { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, int> SocketIdSessionLookup { get; set; }

        int MaxUser { get; set; }
        void OnPlayerJoin(Character ch, ShiftClient client);
        void OnPlayerCreate(IGameObject gameObject);
        void OnObjectDestroy(IGameObject gameObject);
        void OnObjectCreate(IGameObject gameObject);
        void OnObjectMove(ShiftServerData data, ShiftClient client);
        void OnObjectAttack(ShiftServerData data, ShiftClient client);
        void OnObjectUse(ShiftServerData data, ShiftClient client);
        void OnGameStart();
        void BroadcastToRoom(ShiftClient currentClient, MSServerEvent state);
        void BroadcastDataToRoom(ShiftClient currentClient, MSServerEvent state, ShiftServerData data);
        IGroup GetRandomTeam();
        ShiftClient SetRandomNewLeader();
        void OnRoomUpdate();
        void SendRoomState();
    }
}
