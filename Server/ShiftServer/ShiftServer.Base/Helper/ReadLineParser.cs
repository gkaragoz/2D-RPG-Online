using ShiftServer.Base.Core;
using ShiftServer.Proto.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Helper
{
    public static class ReadLineParser
    {
        public class ReadLineResult
        {
            public string command { get; set; }
        }
        public static bool Parse(string userInput)
        {
            var splitedUserInput = userInput.Split(' ');
            var command = splitedUserInput[0];

            if (command == ConsoleCommands.ChangeTickrate)
            {
                int newTickrate = 0;
                try
                {
                    newTickrate = int.Parse(splitedUserInput[1]);
                }
                catch (Exception err)
                {
                    Console.WriteLine("Wrong tickrate !");
                    return true;
                }
                foreach (var room in ServerProvider.instance.world.Rooms.GetValues())
                {
                    room.GameRoomTickRate = newTickrate;
                    room.GameRoomUpdateInterval = TickrateUtil.Set(newTickrate);
                    Console.WriteLine($"{room.Name} >> Updated Tickrate to {newTickrate}!");

                }
                return true;
            }
            else if (command == ConsoleCommands.Quit)
            {
                ServerProvider.instance.Stop();
                return false;

            }
            else if (command == ConsoleCommands.RoomList)
            {
                var roomList = ServerProvider.instance.world.Rooms.GetValues();
                foreach (var room in roomList)
                {
                    Console.WriteLine(string.Format("ID: {0} Room Name: {1} , User: {2}/{3}", room.ID, room.Name, room.Clients.Count, room.MaxUser));
                }
                return true;
            }
            else if (command == ConsoleCommands.PlayerList)
            {
                var playerList = ServerProvider.instance.world.Clients.GetValues();
                var rmList = ServerProvider.instance.world.Rooms.GetValues();

                foreach (var player in playerList)
                {
                    IRoom room = null;
                    if (player.IsJoinedToRoom)
                    {
                        ServerProvider.instance.world.Rooms.TryGetValue(player.JoinedRoomID, out room);
                        Console.WriteLine(string.Format("UserName: {0} #{1}--> Room: {2}", player.UserName, player.ConnectionID, room.Name));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("UserName: {0} #{1}", player.UserName, player.ConnectionID));
                    }
                }
                return true;
            }
            else
            {
                Console.WriteLine(">>> Wrong Command <<<");
                return true;
            }
        }
    }
    public class ConsoleCommands
    {
        public static string Quit = "!q";
        public static string ChangeTickrate = "!tickrate";
        public static string PlayerList = "!players";
        public static string RoomList = "!rooms";
    }
}

