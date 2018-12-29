using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ShiftServer.Client;
namespace ShiftServer.SocketTest
{
    class Program
    {
        private static NetworkClient networkClient = null;

        static void Main(string[] args)
        {
            Console.WriteLine("--- SHIFT SERVER TEST CLIENT ---");
            networkClient = new NetworkClient();
            networkClient.Connect("localhost", 1337);
            networkClient.AddEventListener(ShiftServerMsgID.ShiftServerConnectOk, OnConnected);


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
      
        public static void OnConnected(ShiftServerMsg data)
        {
            Console.WriteLine("OnConnected event triggered::event_id::" + (int)data.MsgId);
        }
    }
}
