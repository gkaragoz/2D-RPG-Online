﻿using Google.Protobuf;
using ShiftServer.Proto.Helper;
using ShiftServer.Base.Auth;
using ShiftServer.Base.Helper;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace ShiftServer.Base.Core
{
    public class ServerProvider
    {
        private static readonly log4net.ILog log
                = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Telepathy.Server server = null;
        public IZone world = null;

        public ServerDataHandler dataHandler = null;
        public Thread listenerThread = null;

        public ServerProvider(IZone createdWorld)
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
                            else
                            {
                                dataHandler.HandleMessage(msg.data, shift, log);

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
                                                if(room.SocketIdSessionLookup.Count == 1) 
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
            shift.SendPacket(MSServerEvent.PingRequest, data);
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
    }
}