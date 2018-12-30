﻿using ShiftServer.Server.Auth;
using ShiftServer.Server.Core;
using ShiftServer.Server.Factory.Entities;
using ShiftServer.Server.Factory.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Worlds
{
    public class RPGWorld : IWorld
    {
        Vector3 IWorld.Scale { get; set; }
        private SafeDictionary<int, IGameObject> GameObjects { get; set; }
        private int ObjectCounter { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }

        public RPGWorld()
        {
            GameObjects = new SafeDictionary<int, IGameObject>();
            Clients = new SafeDictionary<int, ShiftClient>();
        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient shift)
        {

        }

        public void PlayerCreateOnWorld(IGameObject gameObject, ShiftClient shift)
        {
            GameObjects.Add(shift.connectionId, gameObject);
        }

        public void OnPlayerJoin(ShiftServerData data, ShiftClient shift)
        {

            shift.UserSession.SetSid(data);
            //Checking the client has only one player character under control
            IGameObject result = null;
            var alreadyOnGamePlayerObject = GameObjects.TryGetValue(shift.connectionId, out result);

            //remove the player if already exist in world
            if (result != null)
            {
                GameObjects.Remove(shift.connectionId);
            }

            if (data.ClData.Loginname.Length > 20)
            {
                
            }

            Player player = new Player();
            player.OwnerClientSid = shift.UserSession.GetSid();
            player.Name = data.ClData.Loginname;
            player.MaxHP = 100;
            player.CurrentHP = 100;
            player.Position = new Vector3(0, 0, 0);
            player.Rotation = new Vector3(0, 0, 0);
            player.Scale = new Vector3(1, 1, 1);

            this.PlayerCreateOnWorld(player, shift);
            Console.WriteLine("Player joined to world");

            ShiftServerData newData = new ShiftServerData();
            newData.Eid = ServerEventId.SJoinRequestSuccess;
            newData.Session = new SessionData
            {
                Sid = player.OwnerClientSid
            };

            newData.Interaction = new ObjectAction();
            newData.Interaction.CurrentObject = new GameObject
            {
                Oid = player.ObjectId,
                Position = new xVector3 { X = player.Position.X, Y = player.Position.Y, Z = player.Position.Z },
                Rotation = new xVector3 { X = player.Rotation.X, Y = player.Rotation.Y, Z = player.Rotation.Z }
            };

            shift.Send(newData);


        }

        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            MotionMaster.OnMove(data);
        }

        public void OnObjectRemove(ShiftServerData data, ShiftClient shift)
        {

        }

        public void OnWorldUpdate()
        {

        }
    }
}
