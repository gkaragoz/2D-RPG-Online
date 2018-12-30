using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Factory.Entities
{
    class Player : IGameObject
    {
        public string Name { get; set; } = "Gladiator";
        public int ObjectId { get; set; }
        public int MaxHP { get; set; } = 100;
        public int CurrentHP { get; set; } = 100;

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string OwnerClientSid { get; set; }

        public void OnAttack()
        {

        }

        public void OnHit()
        {

        }

        public void OnMove(Vector3 rotation)
        {

        }
    }
}
