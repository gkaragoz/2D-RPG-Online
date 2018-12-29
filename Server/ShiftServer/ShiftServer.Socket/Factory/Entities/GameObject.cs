using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ShiftServer.Server.Factory.Entities
{
    public class GameObject : IGameObject
    {
        public int ObjectId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public void OnAttack()
        {

        }

        public void OnHit()
        {

        }

        public void OnMove()
        {

        }
    }
}
