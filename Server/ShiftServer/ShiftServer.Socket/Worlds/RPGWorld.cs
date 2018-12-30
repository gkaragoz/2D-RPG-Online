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
using ShiftServer.Server.Core;
using System.Threading;

namespace ShiftServer.Server.Worlds
{
    public class RPGWorld : IWorld
    {
        private string Banner = ">>> RPG WORLD <<< \n";
        Vector3 IWorld.Scale { get; set; }

        ServerDataHandler.ObjectPool<int> pool = new ServerDataHandler.ObjectPool<int>(() => new int());

        private SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public int ObjectCounter = 0;

        public RPGWorld()
        {
            GameObjects = new SafeDictionary<int, IGameObject>();
            Clients = new SafeDictionary<int, ShiftClient>();
            Console.WriteLine(Banner);
        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient shift)
        {

        }

        public void OnObjectCreate(IGameObject gameObject)
        {
            GameObjects.Add(Interlocked.Increment(ref ObjectCounter), gameObject);
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

            if (data.ClData == null)
                return;

            if (data.ClData.Loginname.Length > 20)
            {
                
            }

            Player player = new Player();

            player.OwnerClientId = shift.connectionId; 
            player.Name = data.ClData.Loginname;
            player.MaxHP = 100;
            player.CurrentHP = 100;
            player.Position = new Vector3(0, 0, 0);
            player.Rotation = new Vector3(0, 0, 0);
            player.Scale = new Vector3(1, 1, 1);

            this.OnObjectCreate(player);

            Console.WriteLine("Player joined to world");

            ShiftServerData newData = new ShiftServerData();
            newData.Session = new SessionData
            {
                Sid = shift.UserSession.GetSid()
            };
            newData.Interaction = new ObjectAction();
            newData.Interaction.CurrentObject = new sGameObject
            {
                Oid = player.ObjectId,

                PosX = 0,
                PosY = 0,
                PosZ = 0,

                RotX = 0,
                RotY = 0,
                RotZ = 0,

                SclX = 0,
                SclY = 0,
                SclZ = 0
            };

            shift.SendPacket(MSServerEvent.MsJoinRequestSuccess, newData);

        }
        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            Console.WriteLine("An object want to move");
            //MotionMaster.OnMove(data);
        }
        public void OnObjectUse(ShiftServerData data, ShiftClient client)
        {
            Console.WriteLine("An object want to be used");
            ShiftServerData newData = new ShiftServerData();
            newData.Session = new SessionData
            {
                Sid = client.UserSession.GetSid()
            };

            newData.Interaction = new ObjectAction();
            newData.Interaction.CurrentObject = new sGameObject
            {

                PosX = 0,
                PosY = 0,
                PosZ = 0,

                RotX = 0,
                RotY = 0,
                RotZ = 0,

                SclX = 0,
                SclY = 0,
                SclZ = 0
            };

            client.SendPacket(MSPlayerEvent.MsOuse, newData);

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
