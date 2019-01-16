using ShiftServer.Base.Auth;
using ShiftServer.Base.Factory.Movement;
using ShiftServer.Base.Rooms;
using ShiftServer.Proto.Db;
using ShiftServer.Proto.Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public class RoomProvider
    {
        public static RoomProvider instance = null;
        private static readonly log4net.ILog log
                  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ServerProvider _sp = null;
        private SafeDictionary<string, Thread> _roomThreads = new SafeDictionary<string,Thread>();
        public RoomProvider(ServerProvider mainServerProvider)
        {
            instance = this;
            _sp = mainServerProvider;
        }
        public void CreateRoom(IRoom room)
        {
            _sp.world.Rooms.Add(room.Id, room);
            room.IsStopTriggered = false;
            Thread gameRoom = new Thread(room.OnGameStart)
            {
                IsBackground = true,
                Name = "ShiftServer Room Starts " + room.Name
            };
            gameRoom.Start();

            _roomThreads.Add(room.Id, gameRoom);
        }
        
        public void OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {
            if (!_sp.SessionCheck(data, shift))
                return;

            AccountSession session = _sp.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);
            shift.UserName = acc.SelectedCharName;
            shift.UserSession.SetSid(session.SessionID);
            Battleground newRoom = new Battleground(2, 4, _sp.ctx);

            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.Room != null)
                {
                    log.Info($"ClientNO: {shift.connectionId} ------> RoomCreate: " + data.RoomData.Room.Name);

                    data.RoomData.Room.Id = newRoom.Id;
                    newRoom.MaxUser = data.RoomData.Room.MaxUserCount;

                    foreach (var item in newRoom.TeamIdList)
                    {
                        data.RoomData.Room.Teams.Add(item);
                    }

                    newRoom.CreatedUserId = shift.connectionId;
                    newRoom.Name = data.RoomData.Room.Name + " #" + _sp.world.Rooms.Count.ToString();
                    newRoom.CreatedDate = DateTime.UtcNow;
                    newRoom.UpdateDate = DateTime.UtcNow;
                    shift.JoinedRoomId = newRoom.Id;

                    newRoom.ServerLeaderId = shift.connectionId;
                    newRoom.Clients.Add(shift.connectionId, shift);
                    newRoom.SocketIdSessionLookup.Add(data.SessionID, shift.connectionId);

                    data.RoomData.PlayerInfo = new RoomPlayerInfo();
                    data.RoomData.Room.Name = newRoom.Name;

                    IGroup group = newRoom.GetRandomTeam();

                    data.RoomData.PlayerInfo.TeamId = group.Id;
                    data.RoomData.PlayerInfo.Username = acc.SelectedCharName;
                    data.RoomData.PlayerInfo.IsReady = shift.IsReady;
                    data.RoomData.PlayerInfo.ObjectId = shift.CurrentObject.ObjectId;


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
                data.RoomData.Room.CurrentUserCount = newRoom.SocketIdSessionLookup.Count;
                newRoom.MaxConnId = shift.connectionId;

                this.CreateRoom(newRoom);
                shift.SendPacket(MSServerEvent.RoomCreate, data);
                shift.IsJoinedToRoom = true;
                this.SpawnCharacterToRoom(shift, newRoom);
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
            shift.UserName = acc.SelectedCharName;

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

            if (data.RoomData.Room != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.Room.Id, out result);

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
                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.connectionId} ------> Already in a team");
                    }
                    result.Clients.Add(shift.connectionId, shift);
                    result.SocketIdSessionLookup.Add(data.SessionID, shift.connectionId);
                    shift.JoinedRoomId = result.Id;
                    shift.IsJoinedToRoom = true;

                    result.MaxConnId = result.MaxConnId < shift.connectionId ? shift.connectionId : result.MaxConnId;
                    result.BroadcastToRoom(shift, MSServerEvent.RoomPlayerJoined);


                    data.RoomData.Room.Name = result.Name;

                    foreach (var item in result.TeamIdList)
                    {
                        data.RoomData.Room.Teams.Add(item);
                        //listData.RoomData.JoinedRoom.Teams.Add(item);
                    }


                    ShiftClient cl = null;


                    for (int i = 0; i <= result.MaxConnId; i++)
                    {
                        result.Clients.TryGetValue(i, out cl);
                        if (cl != null)
                        {                          

                            AccountSession charSession = _sp.ctx.Sessions.FindBySessionID(cl.UserSession.GetSid());
                            if (charSession == null)
                                continue;

                            Account charAcc = _sp.ctx.Accounts.GetByUserID(charSession.UserID);

                            RoomPlayerInfo pInfo = new RoomPlayerInfo();
                            pInfo.Username = charAcc.SelectedCharName;
                            pInfo.IsReady = cl.IsReady;
                            pInfo.TeamId = cl.JoinedTeamId;
                            pInfo.ObjectId = cl.CurrentObject.ObjectId;
                            if (result.ServerLeaderId == cl.connectionId)
                                pInfo.IsLeader = true;
                            else
                                pInfo.IsLeader = false;

                            data.RoomData.PlayerList.Add(pInfo);
                        }
                    }

                    //if (result.SocketIdSessionLookup.Count > 1)
                    //shift.SendPacket(MSServerEvent.RoomGetPlayers, listData);
                    shift.SendPacket(MSServerEvent.RoomJoin, data);
                    this.SpawnCharacterToRoom(shift, result);
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
                    leavedRoom.RoomData.Room = new MSSRoom();
                    leavedRoom.RoomData.Room.Id = prevRoom.Id;
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
                    else
                        this.OnRoomDispose(prevRoom);



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
                            shift.IsReady = data.RoomData.PlayerReadyStatusInfo.IsReady;

                            ShiftServerData newData = new ShiftServerData();
                            newData.RoomData = new RoomData();
                            newData.RoomData.PlayerReadyStatusInfo = new RoomPlayerInfo();
                            newData.RoomData.PlayerReadyStatusInfo.IsReady = data.RoomData.PlayerReadyStatusInfo.IsReady;
                            newData.RoomData.PlayerReadyStatusInfo.Username = acc.SelectedCharName;

                            result.BroadcastDataToRoom(shift, MSServerEvent.RoomPlayerReadyStatus, newData);
                        }
                    }
                    else
                    {
                        log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                        ShiftServerData errdata = new ShiftServerData();
                        errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                        shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
                    }
                  

                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.connectionId} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
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

            if (data.RoomData.Room != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.Room.Id, out room);

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
                            this.OnRoomDispose(room);
                            _sp.world.Rooms.Remove(data.RoomData.Room.Id);

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
                newData.RoomData.Room = new MSSRoom();
                newData.RoomData.Room.Id = room.Id;
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
                data.RoomData.RoomList.Add(new MSSRoom
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
            //shift.SendPacket(MSServerEvent.LobbyRefresh, data);
        }
        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            MoveInput MoveInput = new MoveInput();
            MoveInput.eventType = data.Plevtid;
            MoveInput.vector3 = new System.Numerics.Vector3(data.PlayerInput.PosX, data.PlayerInput.PosY, data.PlayerInput.PosZ);
            shift.Inputs.Enqueue(MoveInput);
        }

        public void OnRoomDispose(IRoom room)
        {
            try
            {
                room.IsStopTriggered = true;
              
            }
            catch (Exception err)
            {
                log.Error("Error occured when stopping game room", err);
            }
        }
        private void SpawnCharacterToRoom(ShiftClient shift, IRoom room)
        {
            Character character = _sp.ctx.Characters.FindByCharName(shift.UserName);
            room.OnPlayerJoin(character, shift);
        }

     
    }
}
