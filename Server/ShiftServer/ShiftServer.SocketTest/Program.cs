using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ShiftServer.Client;
namespace ShiftServer.SocketTest
{
    class Program
    {
        private static NetworkClient networkClient = null;
        public static int pingRequestedTick;
        public static Thread listenerThread;

        static void Main(string[] args)
        {


            TestFunctions funcs = new TestFunctions();
            Console.WriteLine("--- SHIFT SERVER TEST CLIENT ---");
            networkClient = new NetworkClient();
            networkClient.Connect("localhost", 1337);
            networkClient.AddEventListener(MSServerEvent.MsConnectOk, funcs.OnConnected);
            networkClient.AddEventListener(MSServerEvent.MsPingRequest, funcs.OnPing);



            while (true)
            {
                networkClient.Listen();
                funcs.PingRequestedTick = DateTime.UtcNow.Millisecond;
                networkClient.SendMessage(MSServerEvent.MsPingRequest, null);
                System.Threading.Thread.Sleep(1000);
            }

            //bool runForever = true;
            //while (runForever)
            //{
            //    Console.Write("Command [q cls count]: ");
            //    string userInput = Console.ReadLine();
            //    if (String.IsNullOrEmpty(userInput)) continue;

            //    List<string> clients;

            //    switch (userInput)
            //    {
            //        case "q":
            //            networkClient.Disconnect();
            //            runForever = false;
            //            break;
            //        case "cls":
            //            Console.Clear();
            //            break;


            //    }
            //}

        }


    }

    public class TestFunctions
    {
        public int PingRequestedTick { get; set; }
        public void OnPing(ShiftServerData obj)
        {
            int pongTick = DateTime.UtcNow.Millisecond;
            Console.WriteLine("Ping: " + (pongTick - PingRequestedTick) + " ms");

        }

        public void OnConnected(ShiftServerData data)
        {
            Console.WriteLine("OnConnected event triggered::event_id::" + (int)data.Svevtid);
        }
    }
}
