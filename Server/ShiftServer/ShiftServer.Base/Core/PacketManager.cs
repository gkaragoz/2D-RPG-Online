﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class PacketManager
    {
        public static PacketManager instance = null;
        public PacketManager()
        {
            instance = this;
        }
    }
}
