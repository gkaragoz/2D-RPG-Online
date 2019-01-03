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

        public RoomProvider(ServerProvider mainServerProvider)
        {
            _sp = mainServerProvider;
        }
        public void CreateRoom(IRoom room)
        {
            _sp.world.Rooms.Add(room.Id, room);
        }

        public void OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {

            BattlegroundRoom newRoom = new BattlegroundRoom();
            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.CreatedRoom != null)
                {
                    log.Info($"ClientNO: {shift.connectionId} ------> RoomCreate" + data.RoomData.CreatedRoom.Name);

                    data.RoomData.CreatedRoom.Id = newRoom.Id;
                    newRoom.MaxUser = data.RoomData.CreatedRoom.MaxUserCount;

                    newRoom.CreatedUserId = shift.connectionId;
                    newRoom.Name = data.RoomData.CreatedRoom.Name + " #" + _sp.world.Rooms.Count.ToString();
                    newRoom.CreatedDate = DateTime.UtcNow;
                    newRoom.UpdateDate = DateTime.UtcNow;
                    shift.JoinedRoomId = newRoom.Id;
                    newRoom.ServerLeaderId = shift.connectionId;
                    newRoom.Clients.Add(shift.connectionId, shift);
                    newRoom.SocketIdSessionLookup.Add(shift.UserSession.GetSid(), shift.connectionId);

                }

                data.RoomData.CreatedRoom.CurrentUserCount = newRoom.SocketIdSessionLookup.Count;
                newRoom.MaxConnId = shift.connectionId;
                _sp.world.Rooms.Add(newRoom.Id, newRoom);
                shift.SendPacket(MSServerEvent.RoomCreate, data);
                shift.IsJoinedToRoom = true;
                
                newRoom.BroadcastToRoom(shift, MSSRoomPlayerState.Join);
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.AlreadyInRoom);
                oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                shift.SendPacket(MSServerEvent.RoomCreateFailed, oData);

            }

        }
        public void OnRoomJoin(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;
            IRoom prevRoom = null;
            log.Info($"ClientNO: {shift.connectionId} ------> RoomJoin");
            if (shift.IsJoinedToRoom)
            {
                _sp.world.Rooms.TryGetValue(shift.JoinedRoomId, out prevRoom);
                if (shift.JoinedRoomId == prevRoom.Id)
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.AlreadyInRoom);
                    oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                    shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }

                prevRoom.Clients.Remove(shift.connectionId);
                prevRoom.SocketIdSessionLookup.Remove(shift.UserSession.GetSid());
                prevRoom.BroadcastToRoom(shift, MSSRoomPlayerState.Leaved);

            }


            if (data.RoomData.JoinedRoom != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.JoinedRoom.Id, out result);

                if (result != null)
                {
                    result.Clients.Add(shift.connectionId, shift);
                    result.SocketIdSessionLookup.Add(shift.UserSession.GetSid(), shift.connectionId);
                    shift.JoinedRoomId = result.Id;
                    shift.IsJoinedToRoom = true;
                    result.MaxConnId = result.MaxConnId < shift.connectionId ? shift.connectionId : result.MaxConnId;
                    result.BroadcastToRoom(shift, MSSRoomPlayerState.Join);

                }
                else
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.AlreadyInRoom);
                    oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                    shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }



            }

            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomJoin, data);
        }
        public void OnRoomLeave(ShiftServerData data, ShiftClient shift)
        {
            IRoom prevRoom = null;
            log.Info($"ClientNO: {shift.connectionId} ------> RoomLeave");
            if (shift.IsJoinedToRoom)
            {
                _sp.world.Rooms.TryGetValue(shift.JoinedRoomId, out prevRoom);
                prevRoom.Clients.Remove(shift.connectionId);
                prevRoom.SocketIdSessionLookup.Remove(shift.UserSession.GetSid());
                shift.IsJoinedToRoom = false;
                shift.JoinedRoomId = null;
                ShiftServerData leavedRoom = new ShiftServerData();

                leavedRoom.RoomData = new RoomData();
                leavedRoom.RoomData.LeavedRoom = new MSSRoom();
                leavedRoom.RoomData.LeavedRoom.Id = prevRoom.Id;
                shift.SendPacket(MSServerEvent.RoomLeave, data);
                prevRoom.BroadcastToRoom(shift, MSSRoomPlayerState.Leaved);


            }
            else
            {
                ShiftServerData err = new ShiftServerData();
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomNotFound);
                err.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomLeaveFailed, err);
            }
        }
        public void OnRoomGameStart(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomGameStart");
            _sp.SendMessage(shift.connectionId, MSServerEvent.PingRequest, data);
        }
        public void OnRoomDelete(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomDelete");
            bool isSuccess = false;
            IRoom room = null;
            if (data.RoomData.DeletedRoom != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.DeletedRoom.Id, out room);

                if (room != null)
                {
                    if (room.CreatedUserId == shift.connectionId)
                    {
         
                        if (room.SocketIdSessionLookup.Count > 1)
                        {
                            //_sp.world.Rooms.Remove(data.RoomData.DeletedRoom.Id);
                            for (int i = 0; i < room.MaxConnId; i++)
                            {
                                ShiftClient roomUser = null;
                                room.Clients.TryGetValue(i, out roomUser);

                                if (roomUser != null)
                                {
                                    room.ServerLeaderId = roomUser.connectionId;
                                }
                            }
                      
                        }
                        else
                        {

                            _sp.world.Rooms.Remove(data.RoomData.DeletedRoom.Id);
                          
                        }
                        shift.IsJoinedToRoom = false;
                        isSuccess = true;

                    }
                    else
                    {
                        ShiftServerData errData = new ShiftServerData();
                        errData.ErrorReason = ShiftServerError.RoomAuthProblem;
                        log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem.ToString());
                        shift.SendPacket(MSServerEvent.RoomDeleteFailed, errData);
                        return;
                    }
                }

            }

            if (isSuccess)
            {
                ShiftServerData newData = new ShiftServerData();
                newData.RoomData = new RoomData();
                newData.RoomData.DeletedRoom = new MSSRoom();
                newData.RoomData.DeletedRoom.Id = room.Id;
                shift.SendPacket(MSServerEvent.RoomDelete, newData);
            }
            else
            {
                log.Error($"ClientNO: {shift.connectionId} ------> Room Delete error");
            }

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
                int currentUserCount = room.SocketIdSessionLookup.Count;
                data.RoomData.Rooms.Add(new MSSRoom
                {
                    IsPrivate = room.IsPrivate,
                    IsOwner = room.ServerLeaderId == shift.connectionId,
                    CurrentUserCount = room.SocketIdSessionLookup.Count,
                    MaxUserCount = room.MaxUser,
                    UpdatedTime = room.UpdateDate.Ticks,
                    CreatedTime = room.CreatedDate.Ticks,
                    Name = room.Name,
                    Id = room.Id
                });
            }
            shift.SendPacket(MSServerEvent.LobbyRefresh, data);
        }
    }
}
