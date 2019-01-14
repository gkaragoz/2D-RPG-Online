using Google.Protobuf;
using ShiftServer.Base.Auth;
using ShiftServer.Proto.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class ChatProvider
    {
        private static readonly log4net.ILog log
                = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ChatProvider _cp = null;

        private static Telepathy.Server server = null;
        public IZone world = null;

        public ServerDataHandler dataHandler = null;
        public Thread listenerThread = null;


        public ChatProvider(IZone createdWorld)
        {
            world = createdWorld;
            dataHandler = new ServerDataHandler();
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

            int timerInterval = TickrateUtil.Set(tickrate);


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
                                    IsJoinedToWorld = false,
                                    UserSession = new Session()
                                };
                                //shift.SendPacket(MSServerEvent.Connection, null);
                                world.Clients.Add(msg.connectionId, shift);

                                log.Info("Connected from: " + client.Client.RemoteEndPoint.ToString());
                                break;

                            }
                            catch (Exception err)
                            {
                                log.Error("Error on connection : " + client.Client.RemoteEndPoint.ToString(), err);
                                continue;
                            }


                        case Telepathy.EventType.Data:

                            world.Clients.TryGetValue(msg.connectionId, out shift);
                            int registeredSocketId = 0;
                            string sessionId = shift.UserSession.GetSid();
                            if (sessionId != null)
                            {
                                if (world.SocketIdSessionLookup.TryGetValue(sessionId, out registeredSocketId))
                                {
                                    if (registeredSocketId == msg.connectionId)
                                    {
                                        dataHandler.HandleMessage(msg.data, shift, log);

                                    }
                                    else
                                    {
                                        //possible attack vector
                                        client.Close();
                                    }
                                }
                                else
                                {
                                    //RESTRICTED
                                    dataHandler.HandleMessage(msg.data, shift, log);

                                }
                            }




                            break;
                        case Telepathy.EventType.Disconnected:
                            try
                            {
                                ShiftClient dcedClient = null;
                                string userSessionId = string.Empty;
                                List<ShiftClient> clientList = world.Clients.GetValues();
                                world.Clients.TryGetValue(msg.connectionId, out dcedClient);

                                if (dcedClient.UserSession != null && dcedClient != null)
                                {
                                    userSessionId = dcedClient.UserSession.GetSid();
                                    if (!string.IsNullOrEmpty(userSessionId))
                                    {
                                        IRoom room = null;
                                        if (!string.IsNullOrEmpty(dcedClient.JoinedRoomId))
                                        {
                                            world.Rooms.TryGetValue(dcedClient.JoinedRoomId, out room);

                                            if (room != null)
                                            {
                                                if (room.SocketIdSessionLookup.Count == 1)
                                                {
                                                    // make some other ppl to leader
                                                    room.DisposeInMilliseconds = 50;
                                                }
                                                dcedClient.IsJoinedToRoom = false;
                                                dcedClient.JoinedRoomId = null;
                                                room.Clients.Remove(dcedClient.connectionId);
                                                room.SocketIdSessionLookup.Remove(dcedClient.UserSession.GetSid());
                                            }

                                            IGroup group = null;
                                            room.Teams.TryGetValue(dcedClient.JoinedTeamId, out group);

                                            if (group != null)
                                            {

                                                dcedClient.IsJoinedToRoom = false;
                                                dcedClient.JoinedRoomId = null;
                                                group.Clients.Remove(dcedClient.connectionId);
                                            }


                                        }
                                        world.SocketIdSessionLookup.Remove(userSessionId);
                                    }
                                }
                                if (dcedClient != null)
                                {
                                    world.Clients.Remove(dcedClient.connectionId);

                                }
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

    }
}
