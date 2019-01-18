using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

    public static class Interval
    {
        public static void Set(int timerInterval, Action<object, ElapsedEventArgs> job)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(job);
            aTimer.Interval = timerInterval;
            aTimer.Enabled = true;

        }
    }
   
}
