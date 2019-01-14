using Google.Protobuf.WellKnownTypes;
using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Base.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Rooms
{
    public class Battleground : IRoom
    {

        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        public int MaxUser { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsPrivate { get; set; }
        public int CreatedUserId { get; set; }
        public int ServerLeaderId { get; set; }
        public int DisposeInMilliseconds { get; set; }
        public int MaxConnId { get; set; }
        public int MaxUserPerTeam { get; set; }
        public SafeDictionary<string, IGroup> Teams { get; set; }
        public List<string> TeamIdList { get; set; }
        public string LastActiveTeam { get; set; }

        public Battleground(int groupCount, int maxUserPerTeam)
        {
            MaxUserPerTeam = maxUserPerTeam;

            Clients = new SafeDictionary<int, ShiftClient>();
            SocketIdSessionLookup = new SafeDictionary<string, int>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Teams = new SafeDictionary<string, IGroup>();
            TeamIdList = new List<string>();

            Id = Guid.NewGuid().ToString();
            DisposeInMilliseconds = 10000;

            MaxConnId = 0;

            for (int i = 0; i < groupCount; i++)
            {
                string Id = Guid.NewGuid().ToString();

                Teams.Add(Id, new TeamGroup(Id, MaxUserPerTeam));
                TeamIdList.Add(Id);
            }

        }

        public void OnGameStart(ShiftServerData data, ShiftClient client)
        {

        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnObjectCreate(IGameObject gameObject)
        {
        }

        public void OnObjectDestroy(IGameObject gameObject)
        {
        }

        public void OnObjectMove(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnObjectUse(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerCreate(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerJoin(ShiftServerData data, ShiftClient client)
        {

        }

        public void BroadcastToRoom(ShiftClient currentClient, MSServerEvent evt)
        {
            RoomPlayerInfo pInfo = new RoomPlayerInfo();
            pInfo.Username = currentClient.UserName;

            if (currentClient.JoinedTeamId != null)
                pInfo.TeamId = currentClient.JoinedTeamId;

            pInfo.IsJoinedToTeam = currentClient.IsJoinedToTeam;
            pInfo.IsReady = currentClient.IsReady;

            if (this.ServerLeaderId == currentClient.connectionId)
            {
                pInfo.IsLeader = true;
            }
            else
            {
                pInfo.IsLeader = false;
            }

            ShiftServerData data = new ShiftServerData();
            data.RoomData = new RoomData();
            data.RoomData.PlayerInfo = pInfo;

            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                if (clientList[i].connectionId == currentClient.connectionId)
                    continue;

                clientList[i].SendPacket(evt, data);
            }
        }
        public void BroadcastDataToRoom(ShiftClient currentClient, MSServerEvent state, ShiftServerData data)
        {


            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                if (clientList[i].connectionId == currentClient.connectionId)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }
        public void SendRoomState()
        {

        }

        public void OnRoomUpdate()
        {

        }

        public IGroup GetRandomTeam()
        {
            var RoomTeams = this.Teams.GetValues();

            for (int i = 0; i < RoomTeams.Count; i++)
            {
                if (RoomTeams[i].MaxPlayer > RoomTeams[i].Clients.Count)
                {
                    if (LastActiveTeam != null)
                    {
                        if (LastActiveTeam == RoomTeams[i].Id)
                        {
                            IGroup otherTeam = null;
                            var otherTeamId = TeamIdList.Where(x => x != RoomTeams[i].Id).FirstOrDefault();
                            Teams.TryGetValue(otherTeamId, out otherTeam);
                            if (otherTeam != null)
                            {
                                if (otherTeam.MaxPlayer > otherTeam.Clients.Count)
                                {
                                    LastActiveTeam = otherTeam.Id;
                                    return otherTeam;
                                }
                                else
                                    return null;

                            }
                            else
                            {
                                LastActiveTeam = RoomTeams[i].Id;
                                return RoomTeams[i];
                            }
                        }
                    }
                    else
                    {
                        LastActiveTeam = RoomTeams[i].Id;
                        return RoomTeams[i];
                    }

                }
            }

            return null;
        }

        public ShiftClient SetRandomNewLeader()
        {
            ShiftClient shift = null;
            var AllClients = this.Clients.GetValues();
            foreach (var item in AllClients)
            {
                this.Clients.TryGetValue(item.connectionId, out shift);

                if (shift != null && this.ServerLeaderId == -1)
                {

                    if (this.ServerLeaderId != shift.connectionId)
                    {
                        this.ServerLeaderId = shift.connectionId;
                        shift.IsJoinedToRoom = true;
                        shift.JoinedRoomId = this.Id;
                        return shift;
                    }

                }
            }
         

            return null;
        }
    }
}
