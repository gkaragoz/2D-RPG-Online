using System;
using System.Collections.Generic;

namespace ShiftServer.Proto.Models
{

    [Serializable]
    public class AccountModel
    {
        public bool success;
        public int gem;
        public int gold;
        public string selected_char_name;
        public List<CharacterModel> characters;
    }
}
