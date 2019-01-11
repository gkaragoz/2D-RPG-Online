
using System;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class Character
    {
        public string account_id;
        public string account_email;
        public string name;
        public int class_id;
        public int level;
        public int exp;
    }
}
