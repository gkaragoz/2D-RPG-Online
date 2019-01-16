using Google.Protobuf;
using ShiftServer.Proto.Helper;
using ShiftServer.Base.Auth;
using ShiftServer.Base.Helper;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using ShiftServer.Proto.Db;

namespace ShiftServer.Base.Core
{
    public class ServerProvider
    {
        public static ServerProvider instance = null;
        private static readonly log4net.ILog log
                = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Telepathy.Server server = null;
        public IZone world = null;

        public ServerDataHandler dataHandler = null;
        public Thread listenerThread = null;

        public DBServiceProvider ctx = null;
        public int tickRate = 30;
        public ServerProvider(IZone createdWorld)
        {
            instance = this;
            world = createdWorld;
            dataHandler = new ServerDataHandler();

            try
            {
                ctx = new DBServiceProvider();
            }
            catch (Exception err)
            {

                log.Error("DB PROVIDER FAILED", err);
            }
        }

        public void Listen(int tickrate, int port)
        {
            try
            {
                server = new Telepathy.Server();
                server.NoDelay = true;
                server.SendTimeout = 0;
                server.Start(port);

            }
            catch (Exception err)
            {
                log.Fatal("Server Start Failed", err);
                return;
            }

            log.Info("SERVER PORT: " + port);
            log.Info("SERVER TICK: " + tickrate);
            tickRate = tickrate;
            int timerInterval = TickrateUtil.Set(tickrate);
            SetInterval(timerInterval, UpdateWorld);

            listenerThread = new Thread(GetMessages);
            listenerThread.IsBackground = true;
            listenerThread.Name = "ShiftServer Listener";
            listenerThread.Start();
            log.Info("Server SHIFTDATA Listener Thread Started");


        }

        public void AddServerEventListener(MSServerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            log.Info("Add listener event to: " + eventType.ToString());

            this.dataHandler.serverEvents.Add(new ServerEventCallback
            {
                CallbackFunc = listener,
                EventId = eventType
            });
        }
        public void AddServerEventListener(MSPlayerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            log.Info("Add listener event to: " + eventType.ToString());

            this.dataHandler.playerEvents.Add(new PlayerEventCallback
            {
                CallbackFunc = listener,
                EventId = eventType
            });
        }

        private void UpdateWorld(object source, ElapsedEventArgs e)
        {
            world.OnWorldUpdate();
            world.SendWorldState();
        }

        public void OnMatchmaking(ShiftServerData data, ShiftClient shift)
        {
            //world.OnWorldUpdate();
            //world.SendWorldState();
        }

     
        private static void SetInterval(int timerInterval, Action<object, ElapsedEventArgs> job)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(job);
            // Set the Interval to 1 millisecond.  Note: Time is set in Milliseconds
            aTimer.Interval = timerInterval;
            aTimer.Enabled = true;

        }

        public void GetMessages()
        {
            while (server.Active)
            {
                // grab all new messages. do this in your Update loop.
                Telepathy.Message msg;
                while (server.GetNextMessage(out msg))
                {
                    TcpClient client = server.GetClient(msg.connectionId);
                    ShiftClient shift = null;

                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            try
                            {
                                shift = new ShiftClient
                                {
                                    connectionId = msg.connectionId,
                                    Client = client,
                                    IsJoinedToWorld = true,
                                    IsJoinedToTeam = false,
                                    IsJoinedToRoom = false,
                                    IsReady = false,

                                    UserSession = new Session()
                                };
                                world.Clients.Add(shift.connectionId, shift);
                                if (client != null)
                                {
                                    log.Info("Connected from: " + client.Client.RemoteEndPoint.ToString());
                                    break;

                                }
                                break;
                            }
                            catch (Exception err)
                            {
                                log.Error("Error on connection ", err);
                                continue;
                            }


                        case Telepathy.EventType.Data:
                            world.Clients.TryGetValue(msg.connectionId, out shift);
                            int registeredSocketId = 0;
                        
                            dataHandler.HandleMessage(msg.data, shift, log);
                             
                            break;
                        case Telepathy.EventType.Disconnected:
                            try
                            {
                                ShiftClient dcedClient = null;
                                string userSessionId = string.Empty;
                                List<ShiftClient> clientList = world.Clients.GetValues();

                                world.Clients.TryGetValue(msg.connectionId, out dcedClient);

                                this.DisposeShift(dcedClient);
                                
                                log.Info($"ClientNO: {msg.connectionId} Disconnected");
                            }
                            catch (Exception err)
                            {
                                ShiftServerData errorData = new ShiftServerData();
                                errorData.ErrorReason = ShiftServerError.BadSession;
                                this.SendMessage(msg.connectionId, MSServerEvent.ConnectionLost, errorData);
                                log.Error($"ClientNO: {msg.connectionId} Exception throwed when trying to disconnect", err);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }

        }

        public void DisposeShift(ShiftClient DcedClient)
        {
            string userSessionId = string.Empty;

            if (DcedClient != null)
            {
                userSessionId = DcedClient.UserSession.GetSid();
                if (!string.IsNullOrEmpty(userSessionId))
                {
                    IRoom room = null;
                    if (!string.IsNullOrEmpty(DcedClient.JoinedRoomId))
                    {
                        world.Rooms.TryGetValue(DcedClient.JoinedRoomId, out room);

                        if (room != null)
                        {
                            if (room.SocketIdSessionLookup.Count == 1)
                            {
                                // make some other ppl to leader
                                room.DisposeInMilliseconds = 50;
                            }
                            DcedClient.IsJoinedToRoom = false;
                            DcedClient.JoinedRoomId = null;
                            room.Clients.Remove(DcedClient.connectionId);
                            room.SocketIdSessionLookup.Remove(DcedClient.UserSession.GetSid());
                            bool isDestroyed = false;
                            if (room.Clients.Count == 0)
                            {                               
                                this.world.Rooms.Remove(room.Id);
                                isDestroyed = true;
                            }
                            else
                            {
                                if (DcedClient.CurrentObject != null)
                                    room.GameObjects.Remove(DcedClient.CurrentObject.ObjectId);

                            }

                            if (!isDestroyed)
                                room.BroadcastToRoom(DcedClient, MSServerEvent.RoomPlayerLeft);
                            else
                                RoomProvider.instance.OnRoomDispose(room);

                        }

                        IGroup group = null;
                        room.Teams.TryGetValue(DcedClient.JoinedTeamId, out group);

                        if (group != null)
                        {

                            DcedClient.IsJoinedToRoom = false;
                            DcedClient.JoinedRoomId = null;
                            group.Clients.Remove(DcedClient.connectionId);
                        }


                    }
                    world.SocketIdSessionLookup.Remove(userSessionId);
                }

                world.Clients.Remove(DcedClient.connectionId);
            }
        }
        public void SendDataByConnId(int connId, ShiftServerData data)
        {

            byte[] bb = data.ToByteArray();
            server.Send(connId, bb);
        }

        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(int connId, MSServerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.ServerEvent;
            data.Svevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                server.Send(connId, bb);
        }


        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(int connId, MSPlayerEvent evt, ShiftServerData data)
        {
            data.Basevtid = MSBaseEventId.PlayerEvent;
            data.Plevtid = evt;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                server.Send(connId, bb);
        }


        public void OnPing(ShiftServerData data, ShiftClient shift)
        {
            if (shift != null || shift.Client != null)
                shift.SendPacket(MSServerEvent.PingRequest, data);
        }

        public void OnAccountJoin(ShiftServerData data, ShiftClient shift)
        {
            //if (data.ClientInfo == null)
            //{
            //    ShiftServerData errorData = new ShiftServerData();
            //    errorData.ErrorReason = ShiftServerError.WrongClientData;
            //    log.Warn($"[Failed Login] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");
            //    shift.SendPacket(MSServerEvent.AccountJoinFailed, errorData);
            //    return;
            //}

            try
            {
                if (!this.SessionCheck(data, shift))
                {
                    this.DisposeShift(shift);
                    return;
                }

                if (!this.FillAccountData(data, shift))
                {
                    this.DisposeShift(shift);
                    return;
                }
            }
            catch (Exception err)
            {

                ShiftServerData errorData = new ShiftServerData();
                errorData.ErrorReason = ShiftServerError.NoRespondServer;
                log.Error($"[Failed Login] Internal DB Error Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}", err);
                shift.SendPacket(MSServerEvent.AccountJoinFailed, errorData);
                return;
            }

            string sessionId = shift.UserSession.GetSid();
            data.Session = new SessionData();
            data.Session.Sid = sessionId;

            world.SocketIdSessionLookup.Add(sessionId, shift.connectionId);
            shift.IsJoinedToWorld = true;
            ShiftServerData newData = new ShiftServerData();
            shift.SendPacket(MSServerEvent.Connection, newData);
            log.Info($"[Login Success] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

        }

        public int ClientCount()
        {
            return world.Clients.Count;
        }
        public void Stop()
        {
            // stop the server when you don't need it anymore
            listenerThread.Interrupt();
            if (!listenerThread.Join(2000))
            { // or an agreed resonable time
                listenerThread.Abort();
            }

            server.Stop();
        }

        public bool SessionCheck(ShiftServerData data, ShiftClient shift)
        {
            bool result = false;
            //session check
            if (data.SessionID != null)
            {
                AccountSession session = ctx.Sessions.FindBySessionID(data.SessionID);
                if (session != null)
                {
                    shift.UserSession.SetSid(data.SessionID);
                    Account acc = this.ctx.Accounts.GetByUserID(session.UserID);
                    if (acc != null)
                    {
                        //check if already logged in
                        List<ShiftClient> clients = this.world.Clients.GetValues();
                        var client = clients.Find(x => x.UserSession.GetSid() == session.SessionID);
                        
                        if (session.SessionID == data.SessionID)
                            result = true;

                        if (client != null && shift.IsJoinedToWorld != true)
                            result = false;

                    }
                    else
                    {
                        result = false;
                    }
                 
                }
              
            }


            if (result)
                return result;
            else
            {
                shift.SendPacket(MSServerEvent.ConnectionFailed, new ShiftServerData { ErrorReason = ShiftServerError.BadSession });
                this.DisposeShift(shift);
                return result;
            }
        }
        public bool FillAccountData(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                AccountSession session = this.ctx.Sessions.FindBySessionID(data.SessionID);
                Account acc = this.ctx.Accounts.GetByUserID(session.UserID);
                shift.UserSession.SetSid(data);


                data.Account = null;
                data.AccountData = new CommonAccountData();
                data.AccountData.VirtualMoney = acc.Gold;
                data.AccountData.VirtualSpecialMoney = acc.Gem;

                if (string.IsNullOrEmpty(acc.SelectedCharName))
                {
                    shift.UserName = data.AccountData.Username;
                }                  
                else
                {
                    data.AccountData.Username = acc.SelectedCharName;
                    shift.UserName = acc.SelectedCharName;
                }


                return true;
            }
            catch (Exception err)
            {
                log.Error($"[Login Failed] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}", err);
                return false;
                
            }
        

        }
    }
}
