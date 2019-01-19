using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Proto.Db;
using ShiftServer.Proto.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Rooms
{
    public class Room : IRoom
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int CreatedUserID { get; set; }
        public int MaxConnectionID { get; set; }
        public int RoomLeaderID { get; set; }
        public int TickRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsPersistence { get; set; }
        public bool IsStopTriggered { get; set; }
        public string LastActiveTeam { get; set; }
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<string, IGroup> Teams { get; set; }
        public List<string> TeamIdList { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public int MaxUser { get; set; }
        public TimeSpan CurrentServerUptime { get; set; }
        private RoomPlayerInfo MakeRoomPlayerInfo(ShiftClient currentClient)
        {
            RoomPlayerInfo pInfo = new RoomPlayerInfo();
            pInfo.Username = currentClient.UserName;

            if (currentClient.JoinedTeamID != null)
                pInfo.TeamId = currentClient.JoinedTeamID;

            pInfo.IsJoinedToTeam = currentClient.IsJoinedToTeam;
            pInfo.IsReady = currentClient.IsReady;
            if (currentClient.CurrentObject != null)
            {
                pInfo.ObjectId = currentClient.CurrentObject.ObjectID;
                pInfo.CurrentGObject = new PlayerObject
                {
                    Name = currentClient.UserName,
                    Oid = currentClient.CurrentObject.ObjectID,
                    AttackSpeed = (float)currentClient.CurrentObject.AttackSpeed,
                    MovementSpeed = (float)currentClient.CurrentObject.MovementSpeed,
                    CurrentHp = currentClient.CurrentObject.CurrentHP,
                    MaxHp = currentClient.CurrentObject.MaxHP,
                    PosX = currentClient.CurrentObject.Position.X,
                    PosY = currentClient.CurrentObject.Position.Y,
                    PosZ = currentClient.CurrentObject.Position.Z
                };
            }

            if (this.RoomLeaderID == currentClient.ConnectionID)
                pInfo.IsLeader = true;
            else
                pInfo.IsLeader = false;

            return pInfo;
        }
        public void BroadcastClientState(ShiftClient currentClient, MSServerEvent evt)
        {
            ShiftServerData data = new ShiftServerData();
            data.RoomData = new RoomData();
            data.RoomData.PlayerInfo = MakeRoomPlayerInfo(currentClient);

            this.BroadcastDataToRoom(currentClient, evt, data);
        }
        public IGroup GetRandomTeam()
        {
            var RoomTeams = this.Teams.GetValues();

            for (int i = 0; i < RoomTeams.Count; i++)
            {
                if (!(RoomTeams[i].MaxPlayer > RoomTeams[i].Clients.Count))
                    continue;

                if (LastActiveTeam != null)
                {
                    if (LastActiveTeam != RoomTeams[i].ID)
                        continue;

                    IGroup otherTeam = null;
                    var otherTeamId = TeamIdList.Where(x => x != RoomTeams[i].ID).FirstOrDefault();
                    Teams.TryGetValue(otherTeamId, out otherTeam);
                    if (otherTeam != null)
                    {
                        if (!(otherTeam.MaxPlayer > otherTeam.Clients.Count))
                            return null;

                        LastActiveTeam = otherTeam.ID;
                        return otherTeam;
                    }
                    else
                    {
                        LastActiveTeam = RoomTeams[i].ID;
                        return RoomTeams[i];
                    }

                }
                else
                {
                    LastActiveTeam = RoomTeams[i].ID;
                    return RoomTeams[i];
                }


            }

            return null;
        }
        public void BroadcastDataToRoom(ShiftClient currentClient, MSServerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                if (clientList[i].ConnectionID == currentClient.ConnectionID)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }
        public void BroadcastDataToRoom(ShiftClient currentClient, MSPlayerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                if (clientList[i].ConnectionID == currentClient.ConnectionID)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }
        public ShiftClient SetRandomNewLeader()
        {

            ShiftClient shift = null;
            var AllClients = this.Clients.GetValues();
            foreach (var item in AllClients)
            {
                this.Clients.TryGetValue(item.ConnectionID, out shift);

                if (shift != null && this.RoomLeaderID == -1)
                {

                    if (this.RoomLeaderID != shift.ConnectionID)
                    {
                        this.RoomLeaderID = shift.ConnectionID;
                        shift.IsJoinedToRoom = true;
                        shift.JoinedRoomID = this.ID;
                        return shift;
                    }
                }
            }
            return null;
        }

        public void ClientDispose(ShiftClient client)
        {
            this.Clients.Remove(client.ConnectionID);
            client.IsJoinedToRoom = false;
        }
        public void ClientJoin(ShiftClient client)
        {
            this.Clients.Add(client.ConnectionID, client);
            client.IsJoinedToRoom = true;
        }
        public void OnPlayerJoin(Character character, ShiftClient shift)
        {
        }

        public void OnGameStart()
        {
        }

        public void DisposeClient(ShiftClient client)
        {
            this.Clients.Remove(client.ConnectionID);
        }
    

    }
}
