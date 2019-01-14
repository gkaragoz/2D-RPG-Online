using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Helper
{
    public static class TickRate
    {
        /// <summary>
        /// Calculate tickrate 
        /// </summary>
        /// <param name="mill"></param>
        public static int Calc(int tickrate)
        {
            return (1000 / tickrate);
        }
    }
}
