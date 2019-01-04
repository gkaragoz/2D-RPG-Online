using Google.Protobuf.WellKnownTypes;
using ShiftServer.Server.Auth;
using ShiftServer.Server.Core;
using ShiftServer.Server.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Rooms
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

        public void SendRoomState()
        {
        }

        public void OnRoomUpdate()
        {

        }

        public IGroup GetRandomTeam()
        {
            foreach (var team in this.Teams.GetValues())
            {

                if (team.MaxPlayer != team.Clients.Count)
                {
                    return team;
                }

            }

            return null;
        }
    }
}
