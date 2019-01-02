using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    interface IPlayerGroup
    {
        SafeDictionary<int, IGameObject> GameObjects { get; set; }
        void OnInvite(IGameObject gameObject);
        void OnKick(IGameObject gameObject);
        List<IGameObject> GetGameObjectList();
    }
}
