using ShiftServer.Server.Auth;
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
using System.Threading;

namespace ShiftServer.Server.Worlds
{
    public class RPGWorld : IWorld
    {
        private string Banner = ">>> RPG WORLD <<< \n";
        Vector3 IWorld.Scale { get; set; }

        private SafeDictionary<string, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }

        public int ObjectCounter = 0;

        public RPGWorld()
        {
            GameObjects = new SafeDictionary<string, IGameObject>();
            Clients = new SafeDictionary<int, ShiftClient>();
            Console.WriteLine(Banner);
        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
            
        }

        public void OnObjectCreate(IGameObject gameObject)
        {

            GameObjects.Add(gameObject.ObjectId, gameObject);

        }

        public void OnCreatePlayer(ShiftServerData data, ShiftClient client)
        {
            ShiftServerData newData = new ShiftServerData();
            newData.Session = new SessionData
            {
                Sid = client.UserSession.GetSid()
            };

            IGameObject result = null;

            var dupePlayer = GameObjects.TryGetValue(client.UserSession.GetSid(), out result);

            // if already exist in world
            if (result != null)
            {
                var playerObj = (Player)result;
                newData.PlayerObject = new PlayerObject
                {
                    PClass = playerObj.Class,
                    CurrentHp = playerObj.CurrentHP,
                    MaxHp = playerObj.MaxHP,
                    PObject = new sGameObject
                    {
                        Guid = playerObj.ObjectId,
                        
                        PosX = playerObj.Position.X,
                        PosY = playerObj.Position.Y,
                        PosZ = playerObj.Position.Z
                    }
                };

            }
            else
            {
                Player player = new Player();
                player.OwnerClientId = client.connectionId;
                player.ObjectId = client.UserSession.GetSid();
                player.Name = data.ClData.Loginname;
                player.MaxHP = 100;
                player.CurrentHP = 100;
                player.Position = new Vector3(0, 0, 0);
                player.Rotation = new Vector3(0, 0, 0);
                player.Scale = new Vector3(1, 1, 1);

                this.OnObjectCreate(player);
                Console.WriteLine("Player joined to world");
                newData.PlayerObject = new PlayerObject
                {
                    CurrentHp = player.CurrentHP,
                    MaxHp = player.MaxHP,
                    PClass = player.Class,
                    PObject = new sGameObject
                    {
                        Guid = player.ObjectId,
                        
                        PosX = player.Position.X,
                        PosY = player.Position.Y,
                        PosZ = player.Position.Z,
                    }
                };
            }



            client.SendPacket(MSPlayerEvent.OnCreatePlayer, newData);
        }
        public void OnPlayerJoin(ShiftServerData data, ShiftClient client)
        {

            client.UserSession.SetSid(data);
            //Checking the client has only one player character under control

            Console.WriteLine("Player joined to world");

            ShiftServerData newData = new ShiftServerData();
            newData.Session = new SessionData
            {
                Sid = client.UserSession.GetSid()
            };
            client.SendPacket(MSServerEvent.MsJoinRequestSuccess, newData);

        }
        public void OnObjectMove(ShiftServerData data, ShiftClient client)
        {
            Console.WriteLine("An object want to move");
            //MotionMaster.OnMove(data);
        }
        public void OnObjectUse(ShiftServerData data, ShiftClient client)
        {
            Console.WriteLine("An object want to be used");      

        }
        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectId);
        }

        public void OnWorldUpdate()
        {

        }

        public void SendWorldState()
        {
            var clients = Clients.GetValues();
            foreach (var client in clients)
            {
                if (client.UserSession == null)
                    continue;

                ShiftServerData data = new ShiftServerData();

                client.SendPacket(MSServerEvent.MsWorldUpdate, data);
            }
        }
    }
}
