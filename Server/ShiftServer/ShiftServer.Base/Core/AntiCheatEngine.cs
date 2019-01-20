using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public static class AntiCheatEngine
    {
        public static bool ValidateMoveInput(IGameInput input)
        {
            if (Math.Abs(input.PressTime) > 0.02f)
            {
                return false;
            }
            return true;
        }
    }
}
