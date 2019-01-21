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
        public MSPlayerEvent EventType { get; set; }
        public Vector3 Vector { get; set; }
        public int SequenceID { get; set; }
        public int LastSequenceID { get; set; }
        public float PressTime { get; set; }
    }
}
