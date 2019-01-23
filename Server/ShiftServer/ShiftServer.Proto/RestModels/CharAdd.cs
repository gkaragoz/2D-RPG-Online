using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.RestModels
{
    [Serializable]
    public class CharAdd
    {
        public CharAddError error_type;
        public string error_message;
        public bool success;
        public CharacterModel character;
    }

    public enum CharAddError
    {
        BAD_SESSION = 0,
        CHAR_NAME_TOO_LONG = 1,
        CHAR_NAME_TOO_SHORT = 2,
        CHAR_CLASS_NOT_FOUND = 3,
        CHAR_NAME_ALREADY_EXIST = 4,
        GUESTS_CAN_ONLY_HAVE_ONE_CHAR = 5,
        UNKNOWN_ERROR = 6
    }
}
