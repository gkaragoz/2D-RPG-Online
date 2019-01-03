using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    interface IGroup
    {
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        void OnInvite(ShiftClient client, IGameObject gameObject);
        void OnAccept(ShiftClient client, IGameObject gameObject);
        void OnKick(ShiftClient client, IGameObject gameObject);
    }
}
