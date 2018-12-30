using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IWorld
    {
        Vector3 Scale { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        void OnPlayerJoin(ShiftServerData data, ShiftClient client);
        void OnObjectRemove(ShiftServerData data, ShiftClient client);
        void PlayerCreateOnWorld(IGameObject gameObject, ShiftClient client);
        void OnObjectMove(ShiftServerData data, ShiftClient client);
        void OnObjectAttack(ShiftServerData data, ShiftClient client);
        void OnWorldUpdate();
    }
}
