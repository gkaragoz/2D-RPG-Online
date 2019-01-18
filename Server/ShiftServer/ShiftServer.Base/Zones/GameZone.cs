using ShiftServer.Base.Auth;
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
        public SafeQueue<ShiftClient> MatchMakingPlayerList { get; set; }
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
            log.Debug($"[ATTACK] Remote:{client.Client.Client.RemoteEndPoint.ToString()} ClientNo:{client.ConnectonID}");

            string clientSessionId = client.UserSessionID.GetSid();
            if (clientSessionId == null)
                return;
        }
        public void OnObjectCreate(IGameObject gameObject)
        {
            GameObjects.Add(Interlocked.Increment(ref ObjectCounter), gameObject);
        }
        public void OnPlayerCreate(ShiftServerData data, ShiftClient shift)
        {
            string clientSessionId = shift.UserSessionID;
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();
            Player currentPlayer = (Player)gameObjectList.Where(x => x.OwnerConnectionID == shift.ConnectonID && x.GetType() == typeof(Player)).FirstOrDefault();
            // if already exist in world
            if (currentPlayer != null)
            {
                data.SPlayerObject = new PlayerObject
                {
                    PClass = currentPlayer.Class,
                    CurrentHp = currentPlayer.CurrentHP,
                    MaxHp = currentPlayer.MaxHP,
                    AttackSpeed = (float)currentPlayer.AttackSpeed,
                    MovementSpeed = (float)currentPlayer.MovementSpeed,
                    Oid = currentPlayer.ObjectID,
                    PosX = currentPlayer.Position.X,
                    PosY = currentPlayer.Position.Y,
                    PosZ = currentPlayer.Position.Z
                };


            }
            else
            {
                Player player = new Player();
                player.OwnerConnectionID = shift.ConnectonID;
                player.ObjectID = Interlocked.Increment(ref ObjectCounter);
                player.Name = data.Account.Username;
                player.MaxHP = 100;
                player.CurrentHP = 100;
                player.Position = new Vector3(0, 0, 0);
                player.Rotation = new Vector3(0, 0, 0);
                player.Scale = new Vector3(1, 1, 1);

                this.OnObjectCreate(player);
                log.Info($"[CreatePlayer] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}");

                data.SPlayerObject = new PlayerObject
                {
                    CurrentHp = player.CurrentHP,
                    MaxHp = player.MaxHP,
                    PClass = player.Class,
                    AttackSpeed = (float)player.AttackSpeed,
                    MovementSpeed = (float)player.MovementSpeed,
                    Oid = player.ObjectID,
                    PosX = player.Position.X,
                    PosY = player.Position.Y,
                    PosZ = player.Position.Z,

                };
            }



            shift.SendPacket(MSPlayerEvent.CreatePlayer, data);
        }

        public void OnPlayerDispose(ShiftClient client)
        {

            ServerProvider.instance.world.SocketIdSessionLookup.Remove(client.UserSessionID);
            ServerProvider.instance.world.Clients.Remove(client.ConnectonID);
            if (!string.IsNullOrEmpty(client.UserSessionID))
            {
                IRoom room = null;
                if (!string.IsNullOrEmpty(client.JoinedRoomID))
                {
                    ServerProvider.instance.world.Rooms.TryGetValue(client.JoinedRoomID, out room);

                    if (room != null)
                    {
                        if (room.SocketIdSessionLookup.Count == 1)
                        {
                            // make some other ppl to leader
                            room.DisposeInMilliseconds = 50;
                        }
                        client.IsJoinedToRoom = false;
                        client.JoinedRoomID = null;
                        room.Clients.Remove(client.ConnectonID);
                        room.SocketIdSessionLookup.Remove(client.UserSessionID);
                        bool isDestroyed = false;
                        if (room.Clients.Count == 0)
                        {
                            ServerProvider.instance.world.Rooms.Remove(room.ID);
                            isDestroyed = true;
                        }
                        else
                        {
                            if (client.CurrentObject != null)
                                room.GameObjects.Remove(client.CurrentObject.ObjectID);

                        }

                        IGroup group = null;
                        room.Teams.TryGetValue(client.JoinedTeamID, out group);

                        if (group != null)
                        {

                            client.IsJoinedToRoom = false;
                            client.JoinedRoomID = null;
                            group.Clients.Remove(client.ConnectonID);
                        }

                        if (!isDestroyed)
                            room.BroadcastToRoom(client, MSServerEvent.RoomPlayerLeft);
                        else
                            RoomProvider.instance.OnRoomDispose(room);

                    }


                }

            }

        }
        public void OnPlayerJoin(ShiftServerData data, ShiftClient shift)
        {
            string sessionId = shift.UserSessionID;
            if (sessionId == null)
                return;



        }
        public void OnObjectMove(ShiftServerData data, ShiftClient shift)
        {
            log.Debug($"[MOVE] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}");
        }
        public void OnObjectUse(ShiftServerData data, ShiftClient shift)
        {
            string clientSessionId = shift.UserSessionID;
            if (clientSessionId == null)
                return;
            log.Debug($"[USE]  Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}");

        }
        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectID);
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
                if (clientList[i].UserSessionID == null)
                    continue;

                ShiftServerData data = new ShiftServerData();

                clientList[i].SendPacket(MSPlayerEvent.WorldUpdate, data);
            }

        }
    }
}
