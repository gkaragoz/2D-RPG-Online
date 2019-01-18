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
        private SafeDictionary<string, Thread> _roomThreads = new SafeDictionary<string, Thread>();
        public RoomProvider(ServerProvider mainServerProvider)
        {
            instance = this;
            _sp = mainServerProvider;
        }
        public void CreateRoom(IRoom room)
        {
            _sp.world.Rooms.Add(room.ID, room);
            room.IsStopTriggered = false;
            Thread gameRoom = new Thread(room.OnGameStart)
            {
                IsBackground = true,
                Name = "ShiftServer Room Starts " + room.Name
            };
            gameRoom.Start();

            _roomThreads.Add(room.ID, gameRoom);
        }

        public void OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {
            if (!shift.SessionCheck(data))
                return;

            AccountSession session = DBCont.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);
            shift.UserName = acc.SelectedCharName;
            shift.UserSessionID.SetSid(session.SessionID);
            Battleground newRoom = new Battleground(2, data.RoomData.Room.MaxUserCount, _sp.ctx);

            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.Room != null)
                {
                    log.Info($"ClientNO: {shift.ConnectonID} ------> RoomCreate: " + data.RoomData.Room.Name);

                    data.RoomData.Room.Id = newRoom.ID;
                    newRoom.MaxUser = data.RoomData.Room.MaxUserCount;

                    foreach (var item in newRoom.TeamIdList)
                    {
                        data.RoomData.Room.Teams.Add(item);
                    }

                    newRoom.CreatedUserID = shift.ConnectonID;
                    newRoom.Name = data.RoomData.Room.Name + " #" + _sp.world.Rooms.Count.ToString();
                    newRoom.CreatedDate = DateTime.UtcNow;
                    newRoom.UpdateDate = DateTime.UtcNow;
                    shift.JoinedRoomID = newRoom.ID;

                    newRoom.RoomLeaderID = shift.ConnectonID;
                    newRoom.Clients.Add(shift.ConnectonID, shift);
                    newRoom.SocketIdSessionLookup.Add(data.SessionID, shift.ConnectonID);

                    data.RoomData.PlayerInfo = new RoomPlayerInfo();
                    data.RoomData.Room.Name = newRoom.Name;

                    IGroup group = newRoom.GetRandomTeam();

                    data.RoomData.PlayerInfo.TeamId = group.ID;
                    data.RoomData.PlayerInfo.Username = acc.SelectedCharName;
                    data.RoomData.PlayerInfo.IsReady = shift.IsReady;

                    this.SpawnCharacterToRoom(shift, newRoom);

                    data.RoomData.PlayerInfo.CurrentGObject = new PlayerObject
                    {
                        AttackSpeed = (float)shift.CurrentObject.AttackSpeed,
                        MovementSpeed = (float)shift.CurrentObject.MovementSpeed,
                        CurrentHp = shift.CurrentObject.CurrentHP,
                        MaxHp = shift.CurrentObject.MaxHP,
                        PosX = shift.CurrentObject.Position.X,
                        PosY = shift.CurrentObject.Position.Y,
                        PosZ = shift.CurrentObject.Position.Z
                    };
                    data.RoomData.PlayerInfo.ObjectId = shift.CurrentObject.ObjectID;

                    if (newRoom.RoomLeaderID == shift.ConnectonID)
                        data.RoomData.PlayerInfo.IsLeader = true;
                    else
                        data.RoomData.PlayerInfo.IsLeader = false;

                    data.RoomData.PlayerInfo.IsReady = shift.IsReady;

                    if (shift.JoinTeam(group))
                    {

                        data.RoomData.PlayerInfo.IsJoinedToTeam = shift.IsJoinedToTeam;

                        log.Info($"ClientNO: {shift.ConnectonID} ------> Joined to team");
                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.ConnectonID} ------> Already in a team");
                    }


                }

                newRoom.RoomLeaderID = shift.ConnectonID;
                data.RoomData.Room.CurrentUserCount = newRoom.SocketIdSessionLookup.Count;
                newRoom.MaxConnectionID = shift.ConnectonID;

                this.CreateRoom(newRoom);
                shift.SendPacket(MSServerEvent.RoomCreate, data);

                shift.IsJoinedToRoom = true;
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.AlreadyInRoom);
                oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                shift.SendPacket(MSServerEvent.RoomCreateFailed, oData);

            }

        }
        public void OnRoomJoin(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;
            IRoom prevRoom = null;

            log.Info($"ClientNO: {shift.ConnectonID} ------> RoomJoin");

            if (!_sp.SessionCheck(data, shift))
                return;

            AccountSession session = _sp.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);
            shift.UserName = acc.SelectedCharName;

            if (shift.IsJoinedToRoom)
            {
                _sp.world.Rooms.TryGetValue(shift.JoinedRoomID, out prevRoom);
                if (shift.JoinedRoomID == prevRoom.ID)
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.AlreadyInRoom);
                    oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                    shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }

                IGroup group = shift.GetJoinedTeam(prevRoom);

                if (shift.LeaveFromTeam(prevRoom))
                {
                    log.Info($"ClientNO: {shift.ConnectonID} ------> Left From group");
                }

                prevRoom.Clients.Remove(shift.ConnectonID);
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
                        log.Error($"ClientNO: {shift.ConnectonID} ------> " + ShiftServerError.RoomFull);
                        oData.ErrorReason = ShiftServerError.RoomFull;
                        shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                        return;
                    }
                    IGroup group = result.GetRandomTeam();
                    if (shift.JoinTeam(group))
                    {
                        log.Info($"ClientNO: {shift.ConnectonID} ------> Joined to team");
                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.ConnectonID} ------> Already in a team");
                    }
                    result.Clients.Add(shift.ConnectonID, shift);
                    result.SocketIdSessionLookup.Add(data.SessionID, shift.ConnectonID);
                    shift.JoinedRoomID = result.ID;
                    shift.IsJoinedToRoom = true;

                    result.MaxConnectionID = result.MaxConnectionID < shift.ConnectonID ? shift.ConnectonID : result.MaxConnectionID;


                    data.RoomData.Room.Name = result.Name;

                    foreach (var item in result.TeamIdList)
                    {
                        data.RoomData.Room.Teams.Add(item);
                        //listData.RoomData.JoinedRoom.Teams.Add(item);
                    }


                    ShiftClient cl = null;
                    this.SpawnCharacterToRoom(shift, result);


                    for (int i = 0; i <= result.MaxConnectionID; i++)
                    {
                        result.Clients.TryGetValue(i, out cl);
                        if (cl != null)
                        {

                            AccountSession charSession = _sp.ctx.Sessions.FindBySessionID(cl.UserSessionID.GetSid());

                            if (charSession == null)
                                continue;

                            if (cl.ConnectonID == shift.ConnectonID)
                                continue;

                            Account charAcc = _sp.ctx.Accounts.GetByUserID(charSession.UserID);

                            RoomPlayerInfo pInfo = new RoomPlayerInfo();
                            pInfo.Username = charAcc.SelectedCharName;
                            pInfo.IsReady = cl.IsReady;
                            pInfo.TeamId = cl.JoinedTeamID;
                            pInfo.ObjectId = cl.CurrentObject.ObjectID;
                            pInfo.CurrentGObject = new PlayerObject
                            {
                                Name = cl.CurrentObject.Name,
                                Oid = cl.CurrentObject.ObjectID,
                                AttackSpeed = (float)cl.CurrentObject.AttackSpeed,
                                MovementSpeed = (float)cl.CurrentObject.MovementSpeed,
                                CurrentHp = cl.CurrentObject.CurrentHP,
                                MaxHp = cl.CurrentObject.MaxHP,
                                PosX = cl.CurrentObject.Position.X,
                                PosY = cl.CurrentObject.Position.Y,
                                PosZ = cl.CurrentObject.Position.Z
                            };

                            if (result.RoomLeaderID == cl.ConnectonID)
                                pInfo.IsLeader = true;
                            else
                                pInfo.IsLeader = false;

                            data.RoomData.PlayerList.Add(pInfo);
                        }
                    }

                    //if (result.SocketIdSessionLookup.Count > 1)
                    //shift.SendPacket(MSServerEvent.RoomGetPlayers, listData);
                    data.RoomData.PlayerInfo = new RoomPlayerInfo();
                    data.RoomData.PlayerInfo.CurrentGObject = new PlayerObject
                    {
                        Oid = shift.CurrentObject.ObjectID,
                        Name = shift.CurrentObject.Name,
                        AttackSpeed = (float)shift.CurrentObject.AttackSpeed,
                        MovementSpeed = (float)shift.CurrentObject.MovementSpeed,
                        CurrentHp = shift.CurrentObject.CurrentHP,
                        MaxHp = shift.CurrentObject.MaxHP,
                        PosX = shift.CurrentObject.Position.X,
                        PosY = shift.CurrentObject.Position.Y,
                        PosZ = shift.CurrentObject.Position.Z
                    };
                    data.RoomData.PlayerInfo.ObjectId = shift.CurrentObject.ObjectID;
                    shift.IsJoinedToRoom = true;
                    result.BroadcastToRoom(shift, MSServerEvent.RoomPlayerJoined);
                    shift.SendPacket(MSServerEvent.RoomJoin, data);

                }
                else
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomNotFound);
                    oData.ErrorReason = ShiftServerError.RoomNotFound;
                    shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.AlreadyInRoom);
                oData.ErrorReason = ShiftServerError.RoomNotFound;
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
                log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomAuthProblem);
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
                log.Info($"ClientNO: {shift.ConnectonID} ------> RoomLeave");

                if (!_sp.SessionCheck(data, shift))
                    return;


                if (shift.IsJoinedToRoom)
                {
                    _sp.world.Rooms.TryGetValue(shift.JoinedRoomID, out prevRoom);
                    prevRoom.Clients.Remove(shift.ConnectonID);
                    prevRoom.SocketIdSessionLookup.Remove(data.SessionID);7
                    if (shift.CurrentObject != null)
                        prevRoom.GameObjects.Remove(shift.CurrentObject.ObjectID);

                    shift.CurrentObject = null;
                    shift.IsJoinedToRoom = false;
                    shift.JoinedRoomID = null;
                    ShiftServerData leavedRoom = new ShiftServerData();

                    if (shift.LeaveFromTeam(prevRoom))
                    {
                        log.Info($"ClientNO: {shift.ConnectonID} ------> Left From group");
                    }
                    leavedRoom.RoomData = new RoomData();
                    leavedRoom.RoomData.Room = new MSSRoom();
                    leavedRoom.RoomData.Room.Id = prevRoom.ID;
                    if (prevRoom.Clients.Count == 0 && !prevRoom.IsPersistence)
                    {
                        _sp.world.Rooms.Remove(prevRoom.ID);
                        isDestroyed = true;
                    }
                    else if (shift.ConnectonID == prevRoom.RoomLeaderID)
                    {

                        prevRoom.RoomLeaderID = -1;
                        ShiftClient selectedLeader = prevRoom.SetRandomNewLeader();
                        selectedLeader.IsJoinedToRoom = true;
                        selectedLeader.JoinedRoomID = prevRoom.ID;

                        AccountSession session = _sp.ctx.Sessions.FindBySessionID(selectedLeader.UserSessionID.GetSid());
                        Account acc = _sp.ctx.Accounts.GetByUserID(session.UserID);

                        data.RoomData = new RoomData();
                        RoomPlayerInfo pInfo = new RoomPlayerInfo();
                        pInfo.Username = acc.SelectedCharName;

                        if (selectedLeader.JoinedTeamID != null)
                            pInfo.TeamId = selectedLeader.JoinedTeamID;


                        pInfo.IsJoinedToTeam = selectedLeader.IsJoinedToTeam;
                        pInfo.IsReady = selectedLeader.IsReady;

                        data.RoomData.PlayerInfo = pInfo;

                        if (prevRoom.RoomLeaderID == selectedLeader.ConnectonID)
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
                    log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomNotFound);
                    err.ErrorReason = ShiftServerError.NotInAnyRoom;
                    shift.SendPacket(MSServerEvent.RoomLeaveFailed, err);
                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomLeaveFailed, errdata);
            }

        }
        public void OnRoomGameStart(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectonID} ------> RoomGameStart");


            _sp.SendMessage(shift.ConnectonID, MSServerEvent.RoomGameStart, data);
        }
        public void OnRoomPlayerReadyStatusChanged(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;

            log.Info($"ClientNO: {shift.ConnectonID} ------> PlayerReady StatusChanged");

            if (!shift.SessionCheck(data))
                return;

            AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);

            try
            {

                if (!shift.SessionCheck(data))
                    return;

                if (shift.IsJoinedToRoom)
                {
                    _sp.world.Rooms.TryGetValue(shift.JoinedRoomID, out result);

                    if (result != null && data.RoomData.PlayerReadyStatusInfo != null)
                    {
                        if (shift.JoinedRoomID == result.ID)
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
                        log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomAuthProblem);
                        ShiftServerData errdata = new ShiftServerData();
                        errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                        shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
                    }


                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
            }
            _sp.SendMessage(shift.ConnectonID, MSServerEvent.RoomGameStart, data);
        }
        public void OnRoomLeaderChange(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectonID} ------> RoomLeaderChange");
            _sp.SendMessage(shift.ConnectonID, MSServerEvent.RoomChangeLeader, data);
        }
        public void OnRoomDelete(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectonID} ------> RoomDelete");
            bool isSuccess = false;
            IRoom room = null;

            if (!_sp.SessionCheck(data, shift))
                return;

            if (data.RoomData.Room != null)
            {
                _sp.world.Rooms.TryGetValue(data.RoomData.Room.Id, out room);

                if (room != null)
                {
                    if (room.CreatedUserID == shift.ConnectonID)
                    {

                        if (room.SocketIdSessionLookup.Count > 1)
                        {
                            //_sp.world.Rooms.Remove(data.RoomData.DeletedRoom.Id);
                            for (int i = 0; i < room.MaxConnectionID; i++)
                            {
                                ShiftClient roomUser = null;
                                room.Clients.TryGetValue(i, out roomUser);

                                if (roomUser != null)
                                {
                                    room.RoomLeaderID = roomUser.ConnectonID;
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
                        log.Error($"ClientNO: {shift.ConnectonID} ------>" + ShiftServerError.RoomAuthProblem.ToString());
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
                newData.RoomData.Room.Id = room.ID;
                shift.SendPacket(MSServerEvent.RoomDelete, newData);
            }
            else
            {
                log.Error($"ClientNO: {shift.ConnectonID} ------> Room Delete error");
            }

        }
        public void OnLobbyRefresh(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectonID} ------> LobbyRefresh");

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
                    IsOwner = room.RoomLeaderID == shift.ConnectonID,
                    CurrentUserCount = room.SocketIdSessionLookup.Count,
                    MaxUserCount = room.MaxUser,
                    UpdatedTime = room.UpdateDate.Ticks,
                    CreatedTime = room.CreatedDate.Ticks,
                    Name = room.Name,
                    Id = room.ID
                });
            }
            //shift.SendPacket(MSServerEvent.LobbyRefresh, data);
        }
        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            MoveInput moveInput = new MoveInput();
            moveInput.EventType = data.Plevtid;
            moveInput.Vector = new System.Numerics.Vector3(data.PlayerInput.PosX, data.PlayerInput.PosY, 0);
            log.Debug($"{shift.CurrentObject.ObjectID} wants to move to {moveInput.Vector.ToString()}");

            IRoom room = null;
            _sp.world.Rooms.TryGetValue(shift.JoinedRoomID, out room);
            if (room != null)
            {
                IGameObject go = null;
                room.GameObjects.TryGetValue(shift.CurrentObject.ObjectID, out go);
                if (go != null)
                {
                    moveInput.SequenceID = data.PlayerInput.SequenceID;
                    go.GameInputs.Enqueue(moveInput);
                }
            }
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
