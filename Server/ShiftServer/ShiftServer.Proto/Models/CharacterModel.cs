﻿
using System;

namespace ShiftServer.Proto.Models
{
    [Serializable]
    public class CharacterModel
    {
        public string account_id;
        public string account_email;
        public string name;
        public int class_index;
        public int level;
        public int exp;
    }
}
