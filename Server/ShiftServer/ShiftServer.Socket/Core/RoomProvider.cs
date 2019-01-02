using ShiftServer.Proto.Helper;
using ShiftServer.Server.Auth;
using ShiftServer.Server.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    public class RoomProvider
    {
        private static readonly log4net.ILog log
                  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ServerDataHandler dataHandler = null;
        private ServerProvider _sp = null;

        public RoomProvider(ServerProvider mainServerProvider) {
            _sp = mainServerProvider;
        }
        public void CreateRoom(IRoom room)
        {
            room.Guid = Guid.NewGuid().ToString();
            _sp.world.Rooms.Add(room.Guid, room);
        }

        public void OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {

            BattlegroundRoom newRoom = new BattlegroundRoom();
            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.CreatedRoom != null)
                {
                    log.Info($"ClientNO: {shift.connectionId} ------> RoomCreate" + data.RoomData.CreatedRoom.Name);

                    newRoom.Guid = Guid.NewGuid().ToString();
                    data.RoomData.CreatedRoom.Id = Guid.NewGuid().ToString();
                    newRoom.MaxUser = data.RoomData.CreatedRoom.MaxUserCount;
                    newRoom.Name = data.RoomData.CreatedRoom.Name + " #" + _sp.world.Rooms.Count.ToString();
                    newRoom.CreatedDate = DateTime.UtcNow;
                    newRoom.UpdateDate = DateTime.UtcNow;

                }
                _sp.world.Rooms.Add(newRoom.Guid, newRoom);
                _sp.SendMessage(shift.connectionId, MSServerEvent.RoomCreate, data);
                shift.IsJoinedToRoom = true;
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                oData.ErrorReason = ShiftServerError.RoomNotFound;
                _sp.SendMessage(shift.connectionId, MSServerEvent.RoomJoinFailed, oData);

            }

        }
        public void OnRoomJoin(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;

            log.Info($"ClientNO: {shift.connectionId} ------> RoomJoin");
            if (data.RoomData.JoinedRoom != null)
            { 
                _sp.world.Rooms.TryGetValue(data.RoomData.JoinedRoom.Id, out result);

                if (result != null)
                {
                    result.Clients.Add(shift.connectionId, shift);
                    result.SocketIdSessionLookup.Add(shift.UserSession.GetSid(), shift.connectionId);
                }
                else
                {
                    ShiftServerData oData = new ShiftServerData();
                    oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                    _sp.SendMessage(shift.connectionId, MSServerEvent.RoomJoinFailed, oData);
                    return;
                }



            }

            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomJoin, data);
        }

        public void OnRoomGameStart(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomGameStart");
            _sp.SendMessage(shift.connectionId, MSServerEvent.PingRequest, data);
        }

        public void OnLobbyRefresh(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> LobbyRefresh");
            DateTime now = DateTime.UtcNow;
            //room data
            data.RoomData = new RoomData();
            List<IRoom> svRooms = _sp.world.Rooms.GetValues();
            foreach (var room in svRooms)
            {
                data.RoomData.Rooms.Add(new ServerRoom
                {
                    IsPrivate = room.IsPrivate,
                    CurrentUserCount = room.SocketIdSessionLookup.Count,
                    MaxUserCount = room.MaxUser,
                    UpdatedTime = room.UpdateDate.ToRelativeTime(),
                    CreatedTime = room.CreatedDate.ToRelativeTime(),
                    Name = room.Name,
                    Id = room.Guid
                });
            }
            _sp.SendMessage(shift.connectionId, MSServerEvent.LobbyRefresh, data);
        }
    }
}
