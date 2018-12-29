using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShiftServer.Client.Data.Entities
{

    public enum PacketType
    {

        //PLAYER Entity
        IDLE = 0,
        ATTACK = 1,
        MOVE = 2,

        //LOBBY
        JOIN = 3
        
        
    }
}
