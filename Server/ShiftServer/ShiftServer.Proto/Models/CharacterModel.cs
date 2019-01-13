
using System;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class CharacterModel
    {
        public string account_id;
        public string account_email;
        public string name;
        public int char_class;
        public int level;
        public int exp;
    }
}
