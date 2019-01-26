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

        public UpdateGOList GOUpdatePacket = new UpdateGOList();
        public Battleground(int groupCount, int maxUserPerTeam)
        {
            MaxUserPerTeam = maxUserPerTeam;
            Clients = new SafeDictionary<int, ShiftClient>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Teams = new SafeDictionary<string, IGroup>();
            TeamIdList = new List<string>();
            Scene = PhysxEngine.Engine.CreateScene();
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
            GameRoomUpdateInterval = TickrateUtil.Set(GameRoomTickRate);

            while (!IsStopTriggered)
            {
                this.OnRoomUpdate();
                Thread.Sleep(GameRoomUpdateInterval);
            }


        }
        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
        }
        public void OnPlayerCreate(IGameObject gameObject)
        {
            gameObject.ObjectID = Interlocked.Increment(ref ObjectCounter);
            gameObject.RigidDynamic = PhysxEngine.Engine.CreateRigidDynamic(this.Scene);
            GameObjects.Add(gameObject.ObjectID, gameObject);
            GOUpdatePacket.PlayerList.Add(gameObject.GetPlayerObject());
        }
        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectID);
            var item = GOUpdatePacket.PlayerList.Where(x => x.Id == gameObject.ObjectID).FirstOrDefault();
            GOUpdatePacket.PlayerList.Remove(item);
        }
        public override async void OnPlayerJoinAsync(Character chardata, ShiftClient shift)
        {
            ShiftServerData sendData = new ShiftServerData();
            string clientSessionId = shift.UserSessionID;
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();

            Player player = new Player(chardata, shift);

            this.OnPlayerCreate(player);
            log.Info($"[CreatePlayer] OnRoom:{this.ID} Remote:{shift.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectionID}");

            shift.CurrentObject = player;
            shift.Inputs = new SafeQueue<IGameInput>();
            sendData.RoomData = new RoomData();
            sendData.RoomData.PlayerInfo = new RoomPlayerInfo();
            sendData.RoomData.PlayerInfo.CurrentGObject = shift.CreateNetIdentifierMessage();

            //this.BroadcastDataToRoom(shift, MSPlayerEvent.CreatePlayer, sendData);
            shift.SendPacket(MSPlayerEvent.CreatePlayer, sendData);
        }
        public async void SendRoomStateAsync(TimeSpan timespan)
        {

            ShiftServerData data = new ShiftServerData();
            data.SvTickRate = this.GameRoomTickRate;
            data.GoUpdatePacket = new UpdateGOList();
            data.GoUpdatePacket.PlayerList.Clear();

            IGameObject gObject = null;

            for (int i = 0; i < ObjectCounter + 1; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                if (gObject != null)
                {
                    NetworkIdentifier pObject = gObject.GetPlayerObject();
                    if (pObject != null)
                        data.GoUpdatePacket.PlayerList.Add(pObject);
                }
            }

            this.BroadcastPlayerDataToRoom(MSPlayerEvent.RoomUpdate, data);

        }
        public override void OnRoomUpdate()
        {
            CurrentServerUptime = DateTime.UtcNow - this.GameStartDate;
            TimeSpan updatePassTime = CurrentServerUptime - LastGameUpdate;
            LastGameUpdate = CurrentServerUptime;

            //IGameObject gObject = null;
            //Parallel.For(0, ObjectCounter + 1, i =>
            //  {
            //      GameObjects.TryGetValue(i, out gObject);
            //      if (gObject != null)
            //          gObject.ResolveInputs();
            //  });

            //for (int i = 0; i <= ObjectCounter; i++)
            //{
            //    GameObjects.TryGetValue(i, out gObject);
            //    if (gObject == null)
            //        continue;

            //    gObject.ResolveInputs();
            //}

            SendRoomStateAsync(updatePassTime);
        }
    }
}
