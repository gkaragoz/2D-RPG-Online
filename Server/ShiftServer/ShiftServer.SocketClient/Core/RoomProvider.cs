using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{
    public class RoomProvider
    {
        private List<Room> _roomList;
        public List<Room> RoomList { get => _roomList; private set => _roomList = value; }

        public RoomProvider()
        {
            RoomList = new List<Room>();
        }
        public void AddOrUpdate(ShiftServerData data)
        {
            if (data.RoomData.Rooms != null)
            {
                for (int i = 0; i < data.RoomData.Rooms.Count; i++)
                {
                    bool isNew = true;
                    for (int kk = 0; kk < RoomList.Count; kk++)
                    {
                        if (RoomList[kk].Id == data.RoomData.Rooms[i].Id)
                        {
                            UpdateRoom(kk, data.RoomData.Rooms[i]);
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        Room room = new Room();
                        room.CurrentUser = data.RoomData.Rooms[i].CurrentUserCount;
                        room.MaxUser = data.RoomData.Rooms[i].MaxUserCount;
                        room.Name = data.RoomData.Rooms[i].Name;
                        room.Id = data.RoomData.Rooms[i].Id;

                        AddRoom(room);
                    }

                }
            }
        
        }

        private void AddRoom(Room room)
        {
            RoomList.Add(room);
        }
        private void UpdateRoom(int key, ServerRoom room)
        {
            RoomList[key].CurrentUser = room.CurrentUserCount;
            RoomList[key].MaxUser = room.MaxUserCount;
            RoomList[key].Name = room.Name;
        }
    }

    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxUser { get; set; }
        public int CurrentUser { get; set; }
    }
}
