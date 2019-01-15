using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Factory.Movement
{
    class MoveInput : IGameInput
    {
        public Vector3 vector3 { get; set; }
    }
}
