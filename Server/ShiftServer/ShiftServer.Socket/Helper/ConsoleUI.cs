using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Helper
{
    public static class ConsoleUI
    {
        public static void Run(ServerProvider serverProvider)
        {


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
    }
}
