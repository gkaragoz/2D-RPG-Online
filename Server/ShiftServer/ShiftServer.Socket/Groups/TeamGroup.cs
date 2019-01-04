using ShiftServer.Server.Auth;
using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Groups
{
    public class TeamGroup : IGroup
    {

        public SafeDictionary<int, ShiftClient> Clients { get; set; }

        public int MaxPlayer { get; set; }
        public string Id { get; set; }
        public TeamGroup(string id, int maxPlayer)
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            Id = id;
            MaxPlayer = maxPlayer;
        }
        public void AddPlayer(ShiftClient client)
        {
            client.IsJoinedToTeam = true;
            client.JoinedTeamId = this.Id;
            Clients.Add(client.connectionId, client);
        }
        public void RemovePlayer(ShiftClient client)
        {

            if (client != null)
            {
                client.IsJoinedToTeam = false;
                client.JoinedTeamId = null;
                            
                Clients.Remove(client.connectionId);
            }
            
        }

        public void OnAccept(ShiftClient client, IGameObject gameObject)
        {

        }

        public void OnInvite(ShiftClient client, IGameObject gameObject)
        {

        }

        public void OnKick(ShiftClient client, IGameObject gameObject)
        {

        }

        public void OnLeave(ShiftClient client)
        {

        }

        public void Leave(ShiftClient client)
        {

        }
    }
}
