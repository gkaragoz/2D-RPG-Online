using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IGroup
    {
        string Id { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        int MaxPlayer { get; set; }
        void OnInvite(ShiftClient client, IGameObject gameObject);
        void OnAccept(ShiftClient client, IGameObject gameObject);
        void OnKick(ShiftClient client, IGameObject gameObject);
        void AddPlayer(ShiftClient client);
        void RemovePlayer(ShiftClient client);
        void Leave(ShiftClient client);
    }
}
