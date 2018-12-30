using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static bool SafeDelay(int millisecond)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool flag = false;
            while (!flag)
            {
                if (sw.ElapsedMilliseconds > millisecond)
                {
                    flag = true;
                }
            }
            sw.Stop();
            return true;

        }
    }
}
