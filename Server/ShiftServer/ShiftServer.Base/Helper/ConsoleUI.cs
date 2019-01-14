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
                Console.Write("Command [q cls count roomlist playerlist]: ");

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
                    case "roomlist":
                        var roomList = serverProvider.world.Rooms.GetValues();
                        foreach (var room in roomList)
                        {
                            Console.WriteLine(string.Format("ID: {0} Room Name: {1} , User: {2}/{3}", room.Id, room.Name, room.SocketIdSessionLookup.Count, room.MaxUser));
                        }
                        break;
                    case "playerlist":
                        var playerList = serverProvider.world.Clients.GetValues();
                        var rmList = serverProvider.world.Rooms.GetValues();

                        foreach (var player in playerList)
                        {
                            IRoom room = null;
                            if (player.IsJoinedToRoom)
                            {
                                serverProvider.world.Rooms.TryGetValue(player.JoinedRoomId, out room);
                                Console.WriteLine(string.Format("UserName: {0} #{1}--> Room: {2}", player.UserName, player.connectionId, room.Name));
                            }
                            else
                            {
                                Console.WriteLine(string.Format("UserName: {0} #{1}", player.UserName, player.connectionId));
                            }
                        }
                        break;

                }
            }
        }
    }
}
