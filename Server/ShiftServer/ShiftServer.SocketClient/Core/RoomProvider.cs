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
        public List<Room> RoomList { get; private set; }
        public MSSRoom JoinedRoom { get; private set; }

        public RoomProvider()
        {
            RoomList = new List<Room>();
        }
        public void AddOrUpdate(ShiftServerData data)
        {
            if (data.RoomData.RoomList != null)
            {
                for (int i = 0; i < data.RoomData.RoomList.Count; i++)
                {
                    bool isNew = true;
                    for (int kk = 0; kk < RoomList.Count; kk++)
                    {
                        if (RoomList[kk].Id == data.RoomData.RoomList[i].Id)
                        {
                            UpdateRoom(kk, data.RoomData.RoomList[i]);
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        Room room = new Room();
                        room.CurrentUser = data.RoomData.RoomList[i].CurrentUserCount;
                        room.MaxUser = data.RoomData.RoomList[i].MaxUserCount;
                        room.Name = data.RoomData.RoomList[i].Name;
                        room.Id = data.RoomData.RoomList[i].Id;
                        room.IsOwner = data.RoomData.RoomList[i].IsOwner;
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
                if (data.RoomData.Room != null)
                {
                    JoinedRoom = data.RoomData.Room;
                }
            }
        }
        public void SetCreatedRoom(ShiftServerData data)
        {
            if (data.RoomData != null)
            {
                if (data.RoomData.Room != null)
                {
                    JoinedRoom = data.RoomData.Room;
                }
            }
        }
     
        public void DisposeRoom(ShiftServerData data)
        {
            JoinedRoom = null;
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
        private void UpdateRoom(int key, MSSRoom room)
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
