using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Helper
{
    public static class ConsoleUI
    {
        public static void Run(ServerProvider serverProvider)
        {


            bool runForever = true;

          
            while (runForever)
            {
                Console.Write($" \n" +
                    $"!q - " +
                    $"!tickrate - " +
                    $"!players - " +
                    $"!rooms \n" +
                    $">");


                string userInput = Console.ReadLine();
                if (String.IsNullOrEmpty(userInput)) continue;

                Console.WriteLine("-----------------------------------------");
                ReadLineParser.Parse(userInput);
                Console.WriteLine("-----------------------------------------");


            }
        }
    }
}
