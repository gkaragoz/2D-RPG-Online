using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Helper
{
    public static class TickrateUtil
    {
        /// <summary>
        /// Calculate tickrate 
        /// </summary>
        /// <param name="tickrate"></param>
        public static int Set(int tickrate)
        {
            return (1000 / tickrate);
        }
    }
}
