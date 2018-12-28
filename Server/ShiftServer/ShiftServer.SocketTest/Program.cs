using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ShiftServer.SocketClient;
namespace ShiftServer.SocketTest
{
    class Program
    {
        private static NetworkClient networkClient = null;

        static void Main(string[] args)
        {
            Console.WriteLine("--- SHIFT SERVER TEST CLIENT ---");
            networkClient = new NetworkClient();
            networkClient.Connect();


            //this timer interval simulate the fixed update in unity. must control on server every time
            int timerInterval = 1000/15;

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
                        networkClient.Disconnect();
                        runForever = false;
                        break;
                    case "cls":
                        Console.Clear();
                        break;


                }
            }

        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            networkClient.Receive();
        }
    }
}
