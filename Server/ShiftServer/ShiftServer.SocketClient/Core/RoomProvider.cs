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
        private ServerRoom _joinedRoom;
        public List<Room> RoomList { get => _roomList; private set => _roomList = value; }
        public ServerRoom JoinedRoom { get => _joinedRoom; private set => _joinedRoom = value; }

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
                        room.IsOwner = data.RoomData.Rooms[i].IsOwner;
                        room.IsAvailable = IsRoomAvailableToJoin(room);

                        AddRoom(room);
                    }

                }
            }

        }
        public void SetCurrentJoinedRoom(ShiftServerData data)
        {
            if (data.RoomData != null)
            {
                if (data.RoomData.JoinedRoom != null)
                {
                    JoinedRoom = data.RoomData.JoinedRoom;
                }
            }
        }

        public void DisposeRoom(ShiftServerData data)
        {
            if (data.RoomData != null)
            {
                JoinedRoom = null;
            }
        }

        private void AddRoom(Room room)
        {
            room.IsAvailable = IsRoomAvailableToJoin(room);
            RoomList.Add(room);
        }
        private bool IsRoomAvailableToJoin(Room room)
        {
            return room.IsPrivate || room.IsFull || room.IsOwner ? false : true;
        }
        private void UpdateRoom(int key, ServerRoom room)
        {
            RoomList[key].CurrentUser = room.CurrentUserCount;
            RoomList[key].MaxUser = room.MaxUserCount;
            RoomList[key].Name = room.Name;
            RoomList[key].IsOwner = room.IsOwner;
            RoomList[key].IsAvailable = IsRoomAvailableToJoin(RoomList[key]);
        }
    }

    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsOwner { get; set; }
        public int MaxUser { get; set; }
        public int CurrentUser { get; set; }
        public bool IsFull
        {
            get
            {
                return CurrentUser >= MaxUser;
            }
        }

    }
}
