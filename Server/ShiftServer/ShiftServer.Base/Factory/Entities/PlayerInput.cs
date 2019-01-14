using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Factory.Entities
{
    public class PlayerInput : IGameInput
    {
        public MSPlayerEvent eventType { get; set; }
        public Vector3 vector3 { get; set; }
    }
}
