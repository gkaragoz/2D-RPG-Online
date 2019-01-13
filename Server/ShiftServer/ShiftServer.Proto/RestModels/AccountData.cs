using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class AccountData
    {
        public bool success;
        public int gem;
        public int gold;
        public string selected_char_name;
        public List<CharacterModel> characters;
    }
}
