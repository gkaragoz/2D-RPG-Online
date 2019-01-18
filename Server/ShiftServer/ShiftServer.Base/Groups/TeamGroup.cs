using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Groups
{
    public class TeamGroup : IGroup
    {

        public SafeDictionary<int, ShiftClient> Clients { get; set; }

        public int MaxPlayer { get; set; }
        public string ID { get; set; }
        public TeamGroup(string id, int maxPlayer)
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            ID = id;
            MaxPlayer = maxPlayer;
        }
        public void AddPlayer(ShiftClient client)
        {
            if (MaxPlayer > Clients.Count)
            {
                client.IsJoinedToTeam = true;
                client.JoinedTeamID = this.ID;
                Clients.Add(client.ConnectonID, client);
            }
        }
        public void RemovePlayer(ShiftClient client)
        {

            if (client != null)
            {
                client.IsJoinedToTeam = false;
                client.JoinedTeamID = null;
                            
                Clients.Remove(client.ConnectonID);
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
