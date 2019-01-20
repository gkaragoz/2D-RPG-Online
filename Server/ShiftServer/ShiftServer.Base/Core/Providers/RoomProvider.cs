using ShiftServer.Base.Auth;
using ShiftServer.Base.Factory.Movement;
using ShiftServer.Base.Rooms;
using ShiftServer.Proto.Db;
using ShiftServer.Proto.Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public class RoomProvider
    {
        public static RoomProvider instance = null;

        private static readonly log4net.ILog log
                  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RoomProvider()
        {
            instance = this;
        }
        public void CreateRoom(IRoom room)
        {
            ServerProvider.instance.world.AddRoom(room);
            room.IsStopTriggered = false;
            Thread gameRoom = new Thread(room.OnGameStart)
            {
                IsBackground = true,
                Name = "ShiftServer Room Starts " + room.Name
            };
            gameRoom.Start();
        }

        public async Task OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {
            if (!(await shift.SessionCheckAsync(data)))
                return;

            AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);
            shift.UserName = acc.SelectedCharName;
            shift.UserSessionID = session.SessionID;

            Battleground newRoom = new Battleground(2, data.RoomData.Room.MaxUserCount);

            if (!shift.IsJoinedToRoom)
            {
                if (data.RoomData.Room == null)
                    return;

                log.Info($"ClientNO: {shift.ConnectionID} ------> RoomCreate: " + data.RoomData.Room.Name);

                data.RoomData.Room.Id = newRoom.ID;
                newRoom.MaxUser = data.RoomData.Room.MaxUserCount;

                foreach (var item in newRoom.TeamIdList)
                {
                    data.RoomData.Room.Teams.Add(item);
                }

                newRoom.CreatedUserID = shift.ConnectionID;
                newRoom.Name = data.RoomData.Room.Name + " #" + ServerProvider.instance.world.Rooms.Count.ToString();
                newRoom.CreatedDate = DateTime.UtcNow;
                newRoom.UpdateDate = DateTime.UtcNow;
                shift.JoinedRoomID = newRoom.ID;

                newRoom.RoomLeaderID = shift.ConnectionID;
                newRoom.Clients.Add(shift.ConnectionID, shift);

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

                if (newRoom.RoomLeaderID == shift.ConnectionID)
                    data.RoomData.PlayerInfo.IsLeader = true;
                else
                    data.RoomData.PlayerInfo.IsLeader = false;

                data.RoomData.PlayerInfo.IsReady = shift.IsReady;

                if (shift.JoinTeam(group))
                {

                    data.RoomData.PlayerInfo.IsJoinedToTeam = shift.IsJoinedToTeam;

                    log.Info($"ClientNO: {shift.ConnectionID} ------> Joined to team");
                }
                else
                {
                    log.Info($"ClientNO: {shift.ConnectionID} ------> Already in a team");
                }



                newRoom.RoomLeaderID = shift.ConnectionID;
                data.RoomData.Room.CurrentUserCount = newRoom.Clients.Count;
                newRoom.MaxConnectionID = shift.ConnectionID;

                this.CreateRoom(newRoom);
                await shift.SendPacket(MSServerEvent.RoomCreate, data);

                shift.IsJoinedToRoom = true;
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.AlreadyInRoom);
                oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                await shift.SendPacket(MSServerEvent.RoomCreateFailed, oData);

            }

        }
        public async Task OnRoomJoin(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;
            IRoom prevRoom = null;

            log.Info($"ClientNO: {shift.ConnectionID} ------> RoomJoin");

            if (! await shift.SessionCheckAsync(data))
                return;

            AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);
            shift.UserName = acc.SelectedCharName;

            if (shift.IsJoinedToRoom)
            {
                ServerProvider.instance.world.Rooms.TryGetValue(shift.JoinedRoomID, out prevRoom);
                if (shift.JoinedRoomID == prevRoom.ID)
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.AlreadyInRoom);
                    oData.ErrorReason = ShiftServerError.AlreadyInRoom;
                    await shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }

                IGroup group = shift.GetJoinedTeam(prevRoom);

                if (shift.LeaveFromTeam(prevRoom))
                {
                    log.Info($"ClientNO: {shift.ConnectionID} ------> Left From group");
                }

                prevRoom.Clients.Remove(shift.ConnectionID);
                prevRoom.BroadcastClientState(shift, MSServerEvent.RoomPlayerLeft);
            }

            if (data.RoomData.Room != null)
            {
                ServerProvider.instance.world.Rooms.TryGetValue(data.RoomData.Room.Id, out result);

                if (result != null)
                {
                    if (result.MaxUser == result.Clients.Count)
                    {
                        ShiftServerData oData = new ShiftServerData();
                        log.Error($"ClientNO: {shift.ConnectionID} ------> " + ShiftServerError.RoomFull);
                        oData.ErrorReason = ShiftServerError.RoomFull;
                        await shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                        return;
                    }
                    IGroup group = result.GetRandomTeam();
                    if (shift.JoinTeam(group))
                    {
                        log.Info($"ClientNO: {shift.ConnectionID} ------> Joined to team");
                    }
                    else
                    {
                        log.Info($"ClientNO: {shift.ConnectionID} ------> Already in a team");
                    }
                    result.Clients.Add(shift.ConnectionID, shift);
                    shift.JoinedRoomID = result.ID;
                    shift.IsJoinedToRoom = true;

                    result.MaxConnectionID = result.MaxConnectionID < shift.ConnectionID ? shift.ConnectionID : result.MaxConnectionID;


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

                            AccountSession charSession = DBContext.ctx.Sessions.FindBySessionID(cl.UserSessionID);

                            if (charSession == null)
                                continue;

                            if (cl.ConnectionID == shift.ConnectionID)
                                continue;

                            Account charAcc = DBContext.ctx.Accounts.GetByUserID(charSession.UserID);

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

                            if (result.RoomLeaderID == cl.ConnectionID)
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
                    result.BroadcastClientState(shift, MSServerEvent.RoomPlayerJoined);
                    await shift.SendPacket(MSServerEvent.RoomJoin, data);

                }
                else
                {
                    ShiftServerData oData = new ShiftServerData();
                    log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomNotFound);
                    oData.ErrorReason = ShiftServerError.RoomNotFound;
                    await shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                    return;
                }
            }
            else
            {
                ShiftServerData oData = new ShiftServerData();
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.AlreadyInRoom);
                oData.ErrorReason = ShiftServerError.RoomNotFound;
                await shift.SendPacket(MSServerEvent.RoomJoinFailed, oData);
                return;
            }
        }
        public async Task OnMatchMakingAsync(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                if (! await shift.SessionCheckAsync(data))
                    return;



            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                await shift.SendPacket(MSServerEvent.RoomLeaveFailed, errdata);
            }
        }

        public async Task OnRoomLeave(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                IRoom prevRoom = null;
                bool isDestroyed = false;
                log.Info($"ClientNO: {shift.ConnectionID} ------> RoomLeave");

                if (! await shift.SessionCheckAsync(data))
                    return;


                if (shift.IsJoinedToRoom)
                {
                    ServerProvider.instance.world.Rooms.TryGetValue(shift.JoinedRoomID, out prevRoom);
                    prevRoom.DisposeClient(shift);

                    if (shift.CurrentObject != null)
                        prevRoom.GameObjects.Remove(shift.CurrentObject.ObjectID);

                    shift.CurrentObject = null;

                    ShiftServerData leaveRoomData = new ShiftServerData();

                    if (shift.LeaveFromTeam(prevRoom))
                    {
                        log.Info($"ClientNO: {shift.ConnectionID} ------> Left From group");
                    }
                    leaveRoomData.RoomData = new RoomData();
                    leaveRoomData.RoomData.Room = new MSSRoom();
                    leaveRoomData.RoomData.Room.Id = prevRoom.ID;

                    if (prevRoom.Clients.Count == 0 && !prevRoom.IsPersistence)
                    {
                        ServerProvider.instance.world.Rooms.Remove(prevRoom.ID);
                        isDestroyed = true;
                    }
                    else if (shift.ConnectionID == prevRoom.RoomLeaderID)
                    {

                        prevRoom.RoomLeaderID = -1;
                        ShiftClient selectedLeader = prevRoom.SetRandomNewLeader();
                        selectedLeader.IsJoinedToRoom = true;
                        selectedLeader.JoinedRoomID = prevRoom.ID;

                        AccountSession session = DBContext.ctx.Sessions.FindBySessionID(selectedLeader.UserSessionID);
                        Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);

                        data.RoomData = new RoomData();
                        RoomPlayerInfo pInfo = new RoomPlayerInfo();
                        pInfo.Username = acc.SelectedCharName;

                        if (selectedLeader.JoinedTeamID != null)
                            pInfo.TeamId = selectedLeader.JoinedTeamID;


                        pInfo.IsJoinedToTeam = selectedLeader.IsJoinedToTeam;
                        pInfo.IsReady = selectedLeader.IsReady;
                        data.RoomData.PlayerInfo = pInfo;

                        if (prevRoom.RoomLeaderID == selectedLeader.ConnectionID)
                            pInfo.IsLeader = true;
                        else
                            pInfo.IsLeader = false;

                        prevRoom.BroadcastDataToRoomAsync(shift, MSServerEvent.RoomChangeLeader, data);
                    }

                    await shift.SendPacket(MSServerEvent.RoomLeave, leaveRoomData);

                    if (!isDestroyed)
                        prevRoom.BroadcastClientState(shift, MSServerEvent.RoomPlayerLeft);
                    else
                        this.OnRoomDispose(prevRoom);

                    shift.Dispose();
                }
                else
                {
                    ShiftServerData err = new ShiftServerData();
                    log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomNotFound);
                    err.ErrorReason = ShiftServerError.NotInAnyRoom;
                    await shift.SendPacket(MSServerEvent.RoomLeaveFailed, err);
                }
            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                await shift.SendPacket(MSServerEvent.RoomLeaveFailed, errdata);
            }

        }
        public async Task OnRoomPlayerReadyStatusChangedAsync(ShiftServerData data, ShiftClient shift)
        {
            IRoom result = null;

            log.Info($"ClientNO: {shift.ConnectionID} ------> PlayerReady StatusChanged");

            if (! await shift.SessionCheckAsync(data))
                return;

            AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);
            Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);

            try
            {

                if (! await shift.SessionCheckAsync(data))
                    return;

                if (!shift.IsJoinedToRoom)
                    return;

                ServerProvider.instance.world.Rooms.TryGetValue(shift.JoinedRoomID, out result);

                if (result != null && data.RoomData.PlayerReadyStatusInfo != null)
                {
                    if (shift.JoinedRoomID != result.ID)
                        return;

                    shift.IsReady = data.RoomData.PlayerReadyStatusInfo.IsReady;

                    ShiftServerData newData = new ShiftServerData();
                    newData.RoomData = new RoomData();
                    newData.RoomData.PlayerReadyStatusInfo = new RoomPlayerInfo();
                    newData.RoomData.PlayerReadyStatusInfo.IsReady = data.RoomData.PlayerReadyStatusInfo.IsReady;
                    newData.RoomData.PlayerReadyStatusInfo.Username = acc.SelectedCharName;

                    result.BroadcastDataToRoomAsync(shift, MSServerEvent.RoomPlayerReadyStatus, newData);

                }
                else
                {
                    log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomAuthProblem);
                    ShiftServerData errdata = new ShiftServerData();
                    errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                    await shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
                }



            }
            catch (Exception err)
            {
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomAuthProblem);
                ShiftServerData errdata = new ShiftServerData();
                errdata.ErrorReason = ShiftServerError.NotInAnyRoom;
                await shift.SendPacket(MSServerEvent.RoomPlayerReadyStatusFailed, errdata);
            }
            await shift.SendPacket(MSServerEvent.RoomGameStart, data);
        }
        public async Task OnRoomLeaderChange(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectionID} ------> RoomLeaderChange");
            await shift.SendPacket(MSServerEvent.RoomChangeLeader, data);
        }
        public async Task OnRoomDelete(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectionID} ------> RoomDelete");
            bool isSuccess = false;
            IRoom room = null;

            if (! await shift.SessionCheckAsync(data))
                return;

            if (data.RoomData.Room == null)
                return;

            ServerProvider.instance.world.Rooms.TryGetValue(data.RoomData.Room.Id, out room);

            if (room == null)
                return;

            if (room.CreatedUserID == shift.ConnectionID)
            {

                if (room.Clients.Count > 1)
                {
                    room.SetRandomNewLeader();                   
                }
                else
                {
                    this.OnRoomDispose(room);
                    ServerProvider.instance.world.Rooms.Remove(data.RoomData.Room.Id);

                }
                shift.IsJoinedToRoom = false;
                isSuccess = true;
            }
            else
            {
                ShiftServerData errData = new ShiftServerData();
                errData.ErrorReason = ShiftServerError.RoomAuthProblem;
                log.Error($"ClientNO: {shift.ConnectionID} ------>" + ShiftServerError.RoomAuthProblem.ToString());
                await shift.SendPacket(MSServerEvent.RoomDeleteFailed, errData);
                return;
            }

            if (isSuccess)
            {
                ShiftServerData newData = new ShiftServerData();
                newData.RoomData = new RoomData();
                newData.RoomData.Room = new MSSRoom();
                newData.RoomData.Room.Id = room.ID;
                await shift.SendPacket(MSServerEvent.RoomDelete, newData);

            }
            else
            {
                log.Error($"ClientNO: {shift.ConnectionID} ------> Room Delete error");
            }

        }
        public async Task OnLobbyRefreshAsync(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.ConnectionID} ------> LobbyRefresh");

            if (! await shift.SessionCheckAsync(data))
                return;

            DateTime now = DateTime.UtcNow;
            //room data
            data.RoomData = new RoomData();
            List<IRoom> svRooms = ServerProvider.instance.world.Rooms.GetValues();
            foreach (var room in svRooms)
            {
                int currentUserCount = room.Clients.Count;
                data.RoomData.RoomList.Add(new MSSRoom
                {
                    IsPrivate = room.IsPrivate,
                    IsOwner = room.RoomLeaderID == shift.ConnectionID,
                    CurrentUserCount = room.Clients.Count,
                    MaxUserCount = room.MaxUser,
                    UpdatedTime = room.UpdateDate.Ticks,
                    CreatedTime = room.CreatedDate.Ticks,
                    Name = room.Name,
                    Id = room.ID
                });
            }

            //shift.SendPacket(MSServerEvent.LobbyRefresh, data);
        }
        public async Task OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            MoveInput moveInput = new MoveInput();
            moveInput.EventType = data.Plevtid;
            moveInput.Vector = new System.Numerics.Vector3(data.PlayerInput.PosX, data.PlayerInput.PosY, 0);

            IRoom room = null;
            ServerProvider.instance.world.Rooms.TryGetValue(shift.JoinedRoomID, out room);
            if (room == null)
                return;

            AddGOInput(shift, room, moveInput, data);
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
            Character character = DBContext.ctx.Characters.FindByCharName(shift.UserName);
            room.OnPlayerJoinAsync(character, shift);
        }
        private void AddGOInput(ShiftClient shift, IRoom room, IGameInput input, ShiftServerData data)
        {
            IGameObject go = null;
            room.GameObjects.TryGetValue(shift.CurrentObject.ObjectID, out go);
            if (go != null)
            {
                input.SequenceID = data.PlayerInput.SequenceID;
                go.GameInputs.Enqueue(input);
            }
        }


    }
}
