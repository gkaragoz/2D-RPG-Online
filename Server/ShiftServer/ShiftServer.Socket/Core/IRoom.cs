using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IRoom
    {
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        void OnAccountLogin(ShiftServerData data, ShiftClient client);
        void OnPlayerJoin(ShiftServerData data, ShiftClient client);
        void OnPlayerCreate(ShiftServerData data, ShiftClient client);
        void OnObjectDestroy(IGameObject gameObject);
        void OnObjectCreate(IGameObject gameObject);
        void OnObjectMove(ShiftServerData data, ShiftClient client);
        void OnObjectAttack(ShiftServerData data, ShiftClient client);
        void OnObjectUse(ShiftServerData data, ShiftClient client);
        void OnWorldUpdate();
        void SendWorldState();
    }
}
