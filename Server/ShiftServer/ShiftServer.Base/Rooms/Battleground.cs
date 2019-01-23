using Google.Protobuf.WellKnownTypes;
using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Base.Factory.Entities;
using ShiftServer.Base.Groups;
using ShiftServer.Proto.Db;
using ShiftServer.Proto.Helper;
using ShiftServer.Proto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telepathy;

namespace ShiftServer.Base.Rooms
{
    public class Battleground : Room
    {
        private static readonly log4net.ILog log
          = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DateTime GameStartDate { get; set; }
        public TimeSpan LastGameUpdate { get; set; }
        public int MaxUserPerTeam { get; set; }
        public Vector3 Scale { get; set; }
        public int ObjectCounter = 0;
        public int PlayerCounter = 0;

        public int GameRoomTickRate = 15;

        public UpdateGOList GOUpdatePacket = new UpdateGOList();
        public Battleground(int groupCount, int maxUserPerTeam)
        {
            MaxUserPerTeam = maxUserPerTeam;
            Clients = new SafeDictionary<int, ShiftClient>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Teams = new SafeDictionary<string, IGroup>();
            TeamIdList = new List<string>();

            ID = Guid.NewGuid().ToString();

            MaxConnectionID = 0;

            for (int i = 0; i < groupCount; i++)
            {
                string Id = Guid.NewGuid().ToString();

                Teams.Add(Id, new TeamGroup(Id, MaxUserPerTeam));
                TeamIdList.Add(Id);
            }

            //OnGameStart();


        }
        public override void OnGameStart()
        {
            GameStartDate = DateTime.UtcNow;

            this.StartGameRoom();
        }
        private void StartGameRoom()
        {
            TickRate = GameRoomTickRate;
            int timerInterval = TickrateUtil.Set(TickRate);


            while (!IsStopTriggered)
            {
                this.OnRoomUpdate();
                Thread.Sleep(timerInterval);
            }


        }
        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
        }
        public void OnPlayerCreate(IGameObject gameObject)
        {
            gameObject.ObjectID = Interlocked.Increment(ref ObjectCounter);
            GameObjects.Add(gameObject.ObjectID, gameObject);
            GOUpdatePacket.PlayerList.Add(new PlayerObject
            {
                Name = gameObject.Name,
                MovementSpeed = (float)gameObject.MovementSpeed,
                AttackSpeed = (float)gameObject.AttackSpeed,
                Oid = gameObject.ObjectID,
                PosX = gameObject.Position.X,
                PosY = gameObject.Position.Y,
                PosZ = gameObject.Position.Z,

            });
        }
        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectID);
            var item = GOUpdatePacket.PlayerList.Where(x => x.Oid == gameObject.ObjectID).FirstOrDefault();
            GOUpdatePacket.PlayerList.Remove(item);
        }
        public override async void OnPlayerJoinAsync(Character chardata, ShiftClient shift)
        {
            ShiftServerData sendData = new ShiftServerData();
            string clientSessionId = shift.UserSessionID;
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();
            Player currentPlayer = (Player)gameObjectList.Where(x => x.OwnerConnectionID == shift.ConnectionID && x.GetType() == typeof(Player)).FirstOrDefault();

            // if already exist in world
            if (currentPlayer != null)
            {
                sendData.SPlayerObject = new PlayerObject
                {
                    PClass = currentPlayer.Class,
                    CurrentHp = currentPlayer.CurrentHP,
                    MaxHp = currentPlayer.MaxHP,
                    PosX = currentPlayer.Position.X,
                    PosY = currentPlayer.Position.Y,
                    PosZ = currentPlayer.Position.Z,
                    RotX = currentPlayer.Rotation.X,
                    RotY = currentPlayer.Rotation.Y,
                    RotZ = currentPlayer.Rotation.Z,
                    AttackSpeed = (float)shift.CurrentObject.AttackSpeed,
                    MovementSpeed = (float)shift.CurrentObject.MovementSpeed
                };



            }
            else
            {
                Player player = new Player();
                player.OwnerConnectionID = shift.ConnectionID;
                player.OwnerSessionID = shift.UserSessionID;
                player.Name = chardata.Name;
                player.MaxHP = chardata.Stats.Health;
                player.CurrentHP = chardata.Stats.Health;
                player.AttackSpeed = chardata.Stats.AttackSpeed;
                player.MovementSpeed = chardata.Stats.MovementSpeed;
                player.CurrentHP = chardata.Stats.Health;
                player.Position = new Vector3(0.0f, 0.0f, 0.0f);
                player.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
                player.Scale = new Vector3(1f, 1f, 1f);
                player.State = EntityState.NEWSPAWN;

                this.OnPlayerCreate(player);
                log.Info($"[CreatePlayer] OnRoom:{this.ID} Remote:{shift.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectionID}");


                currentPlayer = player;
            }
            shift.CurrentObject = currentPlayer;
            shift.Inputs = new SafeQueue<IGameInput>();
            sendData.RoomData = new RoomData();
            sendData.RoomData.PlayerInfo = new RoomPlayerInfo();
            sendData.RoomData.PlayerInfo.CurrentGObject = new PlayerObject
            {
                Name = shift.UserName,
                Oid = shift.CurrentObject.ObjectID,
                AttackSpeed = (float)shift.CurrentObject.AttackSpeed,
                MovementSpeed = (float)shift.CurrentObject.MovementSpeed,
                CurrentHp = shift.CurrentObject.CurrentHP,
                MaxHp = shift.CurrentObject.MaxHP,
                PosX = shift.CurrentObject.Position.X,
                PosY = shift.CurrentObject.Position.Y,
                PosZ = shift.CurrentObject.Position.Z
            };

            //this.BroadcastDataToRoom(shift, MSPlayerEvent.CreatePlayer, sendData);
            await shift.SendPacket(MSPlayerEvent.CreatePlayer, sendData);
        }
        public async void SendRoomStateAsync(TimeSpan timespan)
        {

            ShiftServerData data = new ShiftServerData();
            data.SvTickRate = this.TickRate;
            data.GoUpdatePacket = new UpdateGOList();

            IGameObject gObject = null;
            for (int i = 0; i <= ObjectCounter; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                if (gObject == null)
                    continue;


                PlayerObject pObject = gObject.GetPlayerObject();

                data.GoUpdatePacket.PlayerList.Add(pObject);
            }

            await this.BroadcastPlayerDataToRoomAsync(MSPlayerEvent.RoomUpdate, data);
           
        }
        public override void OnRoomUpdate()
        {
            CurrentServerUptime = DateTime.UtcNow - this.GameStartDate;
            TimeSpan updatePassTime = CurrentServerUptime - LastGameUpdate;
            LastGameUpdate = CurrentServerUptime;

            IGameObject gObject = null;
            for (int i = 0; i <= ObjectCounter; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                if (gObject == null)
                    continue;

                gObject.ResolveInputs();
            }

            SendRoomStateAsync(updatePassTime);
        }
    }
}
