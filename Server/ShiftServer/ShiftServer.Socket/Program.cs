using ShiftServer.Proto.Models;
using ShiftServer.Server.Core;
using ShiftServer.Server.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShiftServer.Server
{
    class Program
    {
        private static ServerProvider serverProvider = null;
        /// <summary>
        /// Main entry of shift server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            serverProvider = new ServerProvider();
            serverProvider.AddEventListener(ServerEventId.SJoinRequest, )
            serverProvider.Listen();

            //Run Server Simulation
            int timerInterval = TickrateUtil.Set(15);
            SetInterval(timerInterval, UpdateWorld);
        }
     
        private static void UpdateWorld(object source, ElapsedEventArgs e)
        {
            serverProvider.Update();
        }

        private static void SetInterval(int timerInterval, Action<object, ElapsedEventArgs> updateWorld)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(updateWorld);
            // Set the Interval to 1 millisecond.  Note: Time is set in Milliseconds
            aTimer.Interval = timerInterval;
            aTimer.Enabled = true;
        }



    }
}
