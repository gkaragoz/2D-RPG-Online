using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Factory.Entities
{
    public enum EntityState
    {
        MOVE = 0,
        IDLE = 1,
        STUN = 2,
        GETHIT = 3,
        ATTACK = 4,
        NEWSPAWN = 5
    }
}
