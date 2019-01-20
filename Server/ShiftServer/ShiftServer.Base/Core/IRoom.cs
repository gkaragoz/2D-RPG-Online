using ShiftServer.Base.Auth;
using ShiftServer.Proto.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IRoom
    {
        string Name { get; set; }
        string ID { get; set; }
        int CreatedUserID { get; set; }
        int MaxConnectionID { get; set; }
        int RoomLeaderID { get; set; }
        int TickRate { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdateDate { get; set; }
        TimeSpan CurrentServerUptime { get; set; }
        bool IsPrivate { get; set; }
        bool IsPersistence { get; set; }
        bool IsStopTriggered { get; set; }
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<string, IGroup> Teams { get; set; }
        List<string> TeamIdList { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        int MaxUser { get; set; }
        void BroadcastClientState(ShiftClient currentClient, MSServerEvent state);
        void BroadcastDataToRoomAsync(ShiftClient currentClient, MSServerEvent state, ShiftServerData data);
        IGroup GetRandomTeam();
        ShiftClient SetRandomNewLeader();
        void OnPlayerJoinAsync(Character character, ShiftClient shift);
        void OnGameStart();
        void OnRoomUpdate();
        void DisposeClient(ShiftClient client);
    }
}
