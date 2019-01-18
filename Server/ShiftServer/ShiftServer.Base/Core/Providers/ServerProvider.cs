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
        public int tickRate = 15;
        public int serverMessageTimeout = 1;

        public IZone world = null;

        public Telepathy.Server server = null;
        public ServerEventHandler events = null;
        public Thread listener = null;

        private DBContext ctx = null;
        private ServerCore core = null;

        public static readonly log4net.ILog log
        = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ServerProvider(IZone createdWorld)
        {
            instance = this;
            world = createdWorld;
            events = new ServerEventHandler();
            ctx = new DBContext();
            core = new ServerCore();
        }

        public void Listen(int tickrate, int port)
        {
            try
            {
                server = new Telepathy.Server();
                server.NoDelay = true;
                server.SendTimeout = serverMessageTimeout;
                server.Start(port);
            }
            catch (Exception err)
            {
                log.Fatal("Server Start Failed", err);
                return;
            }

            log.Info("SERVER PORT: " + port);
            log.Info("SERVER TICK: " + tickrate);

            int timerInterval = TickrateUtil.Set(tickrate);

            listener = new Thread(ServerCore.instance.GetMessages);
            listener.IsBackground = true;
            listener.Name = "ShiftServer Listener";
            listener.Start();
            log.Info("Server SHIFTDATA Listener Thread Started");

        }

        public void AddServerEventListener(MSServerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            log.Info("Add listener event to: " + eventType.ToString());

            this.events.serverEvents.Add(new ServerEventCallback
            {
                CallbackFunc = listener,
                EventID = eventType
            });
        }
        public void AddServerEventListener(MSPlayerEvent eventType, Action<ShiftServerData, ShiftClient> listener)
        {
            log.Info("Add listener event to: " + eventType.ToString());

            this.events.playerEvents.Add(new PlayerEventCallback
            {
                CallbackFunc = listener,
                EventID = eventType
            });
        }

        public void OnPing(ShiftServerData data, ShiftClient shift)
        {
            if (shift != null || shift.Client != null)
                shift.SendPacket(MSServerEvent.PingRequest, data);
        }

        public void OnAccountJoin(ShiftServerData data, ShiftClient shift)
        {
            try
            {
                if (!shift.SessionCheck(data))
                {
                    shift.Dispose();
                    return;
                }

                if (!shift.FillAccountData(data))
                {
                    shift.Dispose();
                    return;
                }
            }
            catch (Exception err)
            {

                ShiftServerData errorData = new ShiftServerData();
                errorData.ErrorReason = ShiftServerError.NoRespondServer;
                log.Error($"[Failed Login] Internal DB Error Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}", err);
                shift.SendPacket(MSServerEvent.AccountJoinFailed, errorData);
                return;
            }

            string sessionId = shift.UserSessionID;
            data.Session = new SessionData();
            data.Session.Sid = sessionId;

            world.SocketIdSessionLookup.Add(sessionId, shift.ConnectonID);
            shift.IsJoinedToWorld = true;

            ShiftServerData newData = new ShiftServerData();
            shift.SendPacket(MSServerEvent.Connection, newData);
            log.Info($"[Login Success] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}");

        }

        public int ClientCount()
        {
            return world.Clients.Count;
        }
        public void Stop()
        {
            // stop the server when you don't need it anymore
            listener.Interrupt();
            if (!listener.Join(2000))
            { // or an agreed resonable time
                listener.Abort();
            }

            server.Stop();
        }
    }
}
