using ShiftServer.Base.Auth;
using ShiftServer.Base.Rooms;
using ShiftServer.Proto.Db;
using System;
using System.Collections.Generic;

namespace ShiftServer.Base.Core
{
    public class RoomProvider
    {
        private static readonly log4net.ILog log
                  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            if (!_sp.SessionCheck(data, shift))
                return;

            AccountSession session = _sp.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);


            Battleground newRoom = new Battleground(2, 5);

            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.CreatedRoom != null)
                {
                    log.Info($"ClientNO: {shift.connectionId} ------> RoomCreate: " + data.RoomData.CreatedRoom.Name);

                    data.RoomData.CreatedRoom.Id = newRoom.Id;
                    newRoom.MaxUser = data.RoomData.CreatedRoom.MaxUserCount;

                    foreach (var item in newRoom.TeamIdList)
                    {
                        data.RoomData.CreatedRoom.Teams.Add(item);
                    }

                    newRoom.CreatedUserId = shift.connectionId;
                    newRoom.Name = data.RoomData.CreatedRoom.Name + " #" + _sp.world.Rooms.Count.ToString();
                    data.RoomData.CreatedRoom.Name = newRoom.Name;
                    newRoom.CreatedDate = DateTime.UtcNow;
                    newRoom.UpdateDate = DateTime.UtcNow;
                    shift.JoinedRoomId = newRoom.Id;
                    newRoom.ServerLeaderId = shift.connectionId;
                    newRoom.Clients.Add(shift.connectionId, shift);
                    newRoom.SocketIdSessionLookup.Add(data.SessionID, shift.connectionId);
                    data.RoomData.PlayerInfo = new RoomPlayerInfo();
                    IGroup group = newRoom.GetRandomTeam();

                    data.RoomData.PlayerInfo.TeamId = group.Id;
                    data.RoomData.PlayerInfo.Username = acc.SelectedCharName;
                    data.RoomData.PlayerInfo.IsReady = shift.IsReady;


                    if (newRoom.ServerLeaderId == shift.connectionId)
                        data.RoomData.PlayerInfo.IsLeader = true;
                    else
                        data.RoomData.PlayerInfo.IsLeader = false;

                    data.RoomData.PlayerInfo.IsReady = shift.IsReady;

                    if (shift.JoinTeam(group))
                    {

                        data.RoomData.PlayerInfo.IsJoinedToTeam = shift.IsJoinedToTeam;

                        log.Info($"ClientNO: {shift.connectionId} ------> Joined to team");
                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.connectionId} ------> Already in a team");
                    }


                }

                newRoom.ServerLeaderId = shift.connectionId;
                data.RoomData.CreatedRoom.CurrentUserCount = newRoom.SocketIdSessionLookup.Count;
                newRoom.MaxConnId = shift.connectionId;
                _sp.world.Rooms.Add(newRoom.Id, newRoom);
                shift.SendPacket(MSServerEvent.RoomCreate, data);
                shift.IsJoinedToRoom = true;
                //newRoom.BroadcastToRoom(shift, MSServerEvent.RoomPlayerJoined);
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

            if (!_sp.SessionCheck(data, shift))
                return;

            AccountSession session = _sp.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);

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

                IGroup group = shift.GetJoinedTeam(prevRoom);

                if (shift.LeaveFromTeam(prevRoom))
                {
                    log.Info($"ClientNO: {shift.connectionId} ------> Left From group");
                }

                prevRoom.Clients.Remove(shift.connectionId);
                prevRoom.SocketIdSessionLookup.Remove(data.SessionID);
                prevRoom.BroadcastToRoom(shift, MSServerEvent.RoomPlayerLeft);

            }

            if (data.RoomData.JoinedRoom != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.JoinedRoom.Id, out result);

                if (result != null)
                {
                    if (result.MaxUser == result.SocketIdSessionLookup.Count)
                    {
                        ShiftServerData oData = new ShiftServerData();
                        log.Error($"ClientNO: {shift.connectionId} ------> " + ShiftServerError.AlreadyInRoom);
                        oData.ErrorReason = ShiftServerError.RoomFull;
                        shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                        return;
                    }
                    IGroup group = result.GetRandomTeam();
                    if (shift.JoinTeam(group))
                    {
                        log.Info($"ClientNO: {shift.connectionId} ------> Joined to team");
                        data.RoomData.PlayerInfo = new RoomPlayerInfo();
                        data.RoomData.PlayerInfo.TeamId = group.Id;

                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.connectionId} ------> Already in a team");
                    }
                    result.Clients.Add(shift.connectionId, shift);
                    result.SocketIdSessionLookup.Add(shift.UserSession.GetSid(), shift.connectionId);
                    shift.JoinedRoomId = result.Id;
                    shift.IsJoinedToRoom = true;
                    result.MaxConnId = result.MaxConnId < shift.connectionId ? shift.connectionId : result.MaxConnId;
                    result.BroadcastToRoom(shift, MSServerEvent.RoomPlayerJoined);

                    ShiftServerData listData = new ShiftServerData();
                    listData.RoomData = new RoomData();
                    listData.RoomData.JoinedRoom = new MSSRoom();

                    data.RoomData.JoinedRoom.Name = result.Name;

                    foreach (var item in result.TeamIdList)
                    {
                        data.RoomData.JoinedRoom.Teams.Add(item);
                        listData.RoomData.JoinedRoom.Teams.Add(item);

                    }



                    data.RoomData.PlayerInfo.Username = acc.SelectedCharName;

                    ShiftClient cl = null;


                    for (int i = 0; i <= result.MaxConnId; i++)
                    {
                        result.Clients.TryGetValue(i, out cl);
                        if (cl != null)
                        {
                            if (cl.connectionId == shift.connectionId)
                                continue;

                            RoomPlayerInfo pInfo = new RoomPlayerInfo();
                            pInfo.Username = cl.UserName;
                            pInfo.IsReady = cl.IsReady;
                            pInfo.TeamId = cl.JoinedTeamId;

                            if (result.ServerLeaderId == cl.connectionId)
                                pInfo.IsLeader = true;
                            else
                                pInfo.IsLeader = false;

                            listData.RoomData.PlayerList.Add(pInfo);
                        }
                    }

                    if (result.SocketIdSessionLookup.Count > 1)
                        shift.SendPacket(MSServerEvent.RoomGetPlayers, listData);

                    shift.SendPacket(MSServerEvent.RoomJoin, data);

                }
                else
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomNotFound);
                    oData.ErrorReason = ShiftServerError.RoomNotFound;
                    shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }
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
        public void OnMatchMaking(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                if (!_sp.SessionCheck(data, shift))
                    return;



            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomLeaveFailed, errdata);

            }
        }
        public void OnRoomLeave(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                IRoom prevRoom = null;
                bool isDestroyed = false;
                log.Info($"ClientNO: {shift.connectionId} ------> RoomLeave");

                if (!_sp.SessionCheck(data, shift))
                    return;


                if (shift.IsJoinedToRoom)
                {
                    _sp.world.Rooms.TryGetValue(shift.JoinedRoomId, out prevRoom);
                    prevRoom.Clients.Remove(shift.connectionId);
                    prevRoom.SocketIdSessionLookup.Remove(data.SessionID);
                    shift.IsJoinedToRoom = false;
                    shift.JoinedRoomId = null;
                    ShiftServerData leavedRoom = new ShiftServerData();

                    if (shift.LeaveFromTeam(prevRoom))
                    {
                        log.Info($"ClientNO: {shift.connectionId} ------> Left From group");
                    }
                    leavedRoom.RoomData = new RoomData();
                    leavedRoom.RoomData.LeavedRoom = new MSSRoom();
                    leavedRoom.RoomData.LeavedRoom.Id = prevRoom.Id;
                    if (prevRoom.Clients.Count == 0)
                    {
                        _sp.world.Rooms.Remove(prevRoom.Id);
                        isDestroyed = true;
                    }
                    else if (shift.connectionId == prevRoom.ServerLeaderId)
                    {

                        prevRoom.ServerLeaderId = -1;
                        ShiftClient selectedLeader = prevRoom.SetRandomNewLeader();
                        selectedLeader.IsJoinedToRoom = true;
                        selectedLeader.JoinedRoomId = prevRoom.Id;

                        AccountSession session = _sp.ctx.Sessions.FindBySessionID(selectedLeader.UserSession.GetSid());
                        Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);

                        data.RoomData = new RoomData();
                        RoomPlayerInfo pInfo = new RoomPlayerInfo();
                        pInfo.Username = acc.SelectedCharName;

                        if (selectedLeader.JoinedTeamId != null)
                            pInfo.TeamId = selectedLeader.JoinedTeamId;


                        pInfo.IsJoinedToTeam = selectedLeader.IsJoinedToTeam;
                        pInfo.IsReady = selectedLeader.IsReady;

                        data.RoomData.PlayerInfo = pInfo;

                        if (prevRoom.ServerLeaderId == selectedLeader.connectionId)
                            pInfo.IsLeader = true;
                        else
                            pInfo.IsLeader = false;

                        prevRoom.BroadcastDataToRoom(shift, MSServerEvent.RoomChangeLeader, data);

                    }

                    shift.SendPacket(MSServerEvent.RoomLeave, leavedRoom);

                    if (!isDestroyed)
                        prevRoom.BroadcastToRoom(shift, MSServerEvent.RoomPlayerLeft);


                }
                else
                {
                    ShiftServerData err = new ShiftServerData();
                    log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomNotFound);
                    err.ErrorReason = ShiftServerError.NotInAnyRoom;
                    shift.SendPacket(MSServerEvent.RoomLeaveFailed, err);
                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomLeaveFailed, errdata);
            }

        }
        public void OnRoomGameStart(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomGameStart");
            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomGameStart, data);
        }

        public void OnRoomPlayerReadyStatusChanged(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;

            log.Info($"ClientNO: {shift.connectionId} ------> PlayerReady StatusChanged");

            if (!_sp.SessionCheck(data, shift))
                return;

            AccountSession session = _sp.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);

            try
            {

                if (!_sp.SessionCheck(data, shift))
                    return;

                if (shift.IsJoinedToRoom)
                {
                    _sp.world.Rooms.TryGetValue(shift.JoinedRoomId, out result);

                    if (result != null && data.RoomData.PlayerReadyStatusInfo != null)
                    {
                        if (shift.JoinedRoomId == result.Id)
                        {
                            shift.IsReady = true;

                            ShiftServerData newData = new ShiftServerData();
                            newData.RoomData = new RoomData();
                            newData.RoomData.PlayerReadyStatusInfo = new RoomPlayerInfo();
                            newData.RoomData.PlayerReadyStatusInfo.IsReady = data.RoomData.PlayerReadyStatusInfo.IsReady;
                            newData.RoomData.PlayerReadyStatusInfo.Username = acc.SelectedCharName;

                            result.BroadcastDataToRoom(shift, MSServerEvent.RoomPlayerReadyStatusChanged, newData);
                        }
                    }
                    else
                    {
                        log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                        ShiftServerData errdata = new ShiftServerData();
                        errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                        shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusChangedFailed, errdata);
                    }
                  

                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusChangedFailed, errdata);
            }
            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomGameStart, data);
        }
        public void OnRoomLeaderChange(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomLeaderChange");
            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomChangeLeader, data);
        }
        public void OnRoomDelete(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomDelete");
            bool isSuccess = false;
            IRoom room = null;

            if (!_sp.SessionCheck(data, shift))
                return;

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

            if (!_sp.SessionCheck(data, shift))
                return;

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
