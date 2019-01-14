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
    public class WorldGroup : IGroup
    {
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public string Id { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public int MaxPlayer { get; set; }

        public void AddPlayer(ShiftClient client)
        {
        }

        public void Leave(ShiftClient client)
        {
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

        public void RemovePlayer(ShiftClient client)
        {
        }
    }
}
