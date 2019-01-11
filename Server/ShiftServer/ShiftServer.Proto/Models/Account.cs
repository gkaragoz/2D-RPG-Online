using System;
using System.Collections.Generic;

namespace ShiftServer.Proto.Models
{

    [Serializable]
    public class Account
    {
        public bool success;
        public int gem;
        public int gold;
        public List<Character> characters;
    }
}
