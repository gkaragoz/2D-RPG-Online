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
            serverProvider.Listen();


            
            int timerInterval = TickRate.Calc(15);


            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 1 millisecond.  Note: Time is set in Milliseconds
            aTimer.Interval = timerInterval;
            aTimer.Enabled = true;



            bool runForever = true;
            while (runForever)
            {
                Console.Write("Command [q cls count]: ");
                string userInput = Console.ReadLine();
                if (String.IsNullOrEmpty(userInput)) continue;

                List<string> clients;

                switch (userInput)
                {
                    case "q":
                        serverProvider.Stop();
                        runForever = false;
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "count":
                        int count = serverProvider.ClientCount();
                        Console.WriteLine("Total user : " + count);
                        break;
                      
                }
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            serverProvider.Update();
        }

    }
}
