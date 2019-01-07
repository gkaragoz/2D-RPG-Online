﻿using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Base.Factory.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Telepathy;
using System.Threading;

namespace ShiftServer.Base.Worlds
{
    public class GameZone : IZone
    {
        private static readonly log4net.ILog log
             = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Vector3 IZone.Scale { get; set; }

        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<string, IRoom> Rooms { get; set; }
        public SafeDictionary<string, Thread> RoomGameThreadList { get; set; }

        public int ObjectCounter = 0;
        public int PlayerCounter = 0;

        public GameZone()
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            SocketIdSessionLookup = new SafeDictionary<string, int>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Rooms = new SafeDictionary<string, IRoom>();
            RoomGameThreadList = new SafeDictionary<string, Thread>();
        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
            log.Debug($"[ATTACK] Remote:{client.Client.Client.RemoteEndPoint.ToString()} ClientNo:{client.connectionId}");

            string clientSessionId = client.UserSession.GetSid();
            if (clientSessionId == null)
                return;
        }

        public void OnObjectCreate(IGameObject gameObject)
        {
            GameObjects.Add(Interlocked.Increment(ref ObjectCounter), gameObject);
        }

        public void OnPlayerCreate(ShiftServerData data, ShiftClient shift)
        {
            string clientSessionId = shift.UserSession.GetSid();
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();
            Player currentPlayer = (Player)gameObjectList.Where(x => x.OwnerConnectionId == shift.connectionId && x.GetType() == typeof(Player)).FirstOrDefault();
            // if already exist in world
            if (currentPlayer != null)
            {
                data.SPlayerObject = new PlayerObject
                {
                    PClass = currentPlayer.Class,
                    CurrentHp = currentPlayer.CurrentHP,
                    MaxHp = currentPlayer.MaxHP,
                    PObject = new sGameObject
                    {
                        Oid = currentPlayer.ObjectId,                      
                        PosX = currentPlayer.Position.X,
                        PosY = currentPlayer.Position.Y,
                        PosZ = currentPlayer.Position.Z
                    }
                };
                

            }
            else
            {
                Player player = new Player();
                player.OwnerConnectionId = shift.connectionId;
                player.ObjectId = Interlocked.Increment(ref ObjectCounter);
                player.Name = data.Account.Username;
                player.MaxHP = 100;
                player.CurrentHP = 100;
                player.Position = new Vector3(0, 0, 0);
                player.Rotation = new Vector3(0, 0, 0);
                player.Scale = new Vector3(1, 1, 1);

                this.OnObjectCreate(player);
                log.Info($"[CreatePlayer] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

                data.SPlayerObject = new PlayerObject
                {
                    CurrentHp = player.CurrentHP,
                    MaxHp = player.MaxHP,
                    PClass = player.Class,
                    PObject = new sGameObject
                    {
                        Oid = player.ObjectId,

                        PosX = player.Position.X,
                        PosY = player.Position.Y,
                        PosZ = player.Position.Z,
                    }
                };
            }



            shift.SendPacket(MSPlayerEvent.CreatePlayer, data);
        }
        public void OnPlayerJoin(ShiftServerData data, ShiftClient shift)
        {
            string sessionId = shift.UserSession.GetSid();
            if (sessionId == null)
                return;



        }
        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            string clientSessionId = shift.UserSession.GetSid();
            if (clientSessionId == null)
                return;
            log.Debug($"[MOVE] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");
        }
        public void OnObjectUse(ShiftServerData data, ShiftClient shift)
        {
            string clientSessionId = shift.UserSession.GetSid();
            if (clientSessionId == null)
                return;
            log.Debug($"[USE]  Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

        }
        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectId);
        }
        public void OnWorldUpdate()
        {
            IGameObject gObject = null;
            for (int i = 0; i < ObjectCounter; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                IGameInput gInput = null;
                PlayerInput pInput = null;
                for (int kk = 0; kk < gObject.GameInputs.Count; kk++)
                {
                    gObject.GameInputs.TryDequeue(out gInput);
                    //pInput = (PlayerInput)gInput;
                }
            }
        }

        public void SendWorldState()
        {
            List<ShiftClient> clientList = Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                ShiftServerData data = new ShiftServerData();

                clientList[i].SendPacket(MSPlayerEvent.WorldUpdate, data);
            }
  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///
        /// data.Session;
        /// data.AccountData;
        /// </returns>
        /// <param name="data"></param>
        /// <param name="shift"></param>
        public void OnAccountLogin(ShiftServerData data, ShiftClient shift)
        {
            if (data.ClientInfo == null)
            {
                ShiftServerData errorData = new ShiftServerData();
                errorData.ErrorReason = ShiftServerError.WrongClientData;
                log.Warn($"[Failed Login] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");
                shift.SendPacket(MSServerEvent.LoginFailed, errorData);
                return;
            }

          
            //check account
            string accUsername = data.Account.Username;
            string accPassword = data.Account.Password;
            //QUERY TO SOMEWHERE ELSE

            //Checking the client has only one player character under control
            shift.UserSession.SetSid(data);
            string sessionId = shift.UserSession.GetSid();
            data.Session = new SessionData();
            data.Session.Sid = sessionId;
            //check login data

            data.Account = null;
            data.AccountData = new CommonAccountData();
            data.AccountData.Username = accUsername;
            data.AccountData.VirtualMoney = 100;
            data.AccountData.VirtualSpecialMoney = 100;

            shift.SendPacket(MSServerEvent.Login, data);
            shift.UserName = data.AccountData.Username;
            SocketIdSessionLookup.Add(sessionId, shift.connectionId);
            log.Info($"[Login Success] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

        }
    }
}