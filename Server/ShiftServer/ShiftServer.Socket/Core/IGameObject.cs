using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Core
{
    public interface IGameObject
    {
        string Name { get; set; }
        int OwnerClientId { get; set; }
        int ObjectId { get; set; }
        int MaxHP { get; set; }
        int CurrentHP { get; set; }
        List<IGameObject> OwnedObjects { get; set; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }

        void OnAttack();
        void OnHit();
        void OnMove(Vector3 rotation);

    }
}
