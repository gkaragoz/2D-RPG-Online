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
    public class Battleground : IRoom
    {
        private static readonly log4net.ILog log
          = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        public int MaxUser { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime GameStartDate { get; set; }
        public TimeSpan LastGameUpdate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsPersistence { get; set; }
        public bool IsStopTriggered { get; set; }
        public int CreatedUserID { get; set; }
        public int RoomLeaderID { get; set; }
        public int DisposeInMilliseconds { get; set; }
        public int MaxConnectionID { get; set; }
        public int MaxUserPerTeam { get; set; }
        public SafeDictionary<string, IGroup> Teams { get; set; }
        public List<string> TeamIdList { get; set; }
        public string LastActiveTeam { get; set; }
        public int TickRate { get; set; }
        public TimeSpan CurrentServerUptime { get; set; }

        public int ObjectCounter = 0;
        public int PlayerCounter = 0;

        public UpdateGOList GOUpdatePacket = new UpdateGOList();


        public Battleground(int groupCount, int maxUserPerTeam)
        {
            MaxUserPerTeam = maxUserPerTeam;
            Clients = new SafeDictionary<int, ShiftClient>();
            SocketIdSessionLookup = new SafeDictionary<string, int>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Teams = new SafeDictionary<string, IGroup>();
            TeamIdList = new List<string>();

            ID = Guid.NewGuid().ToString();
            DisposeInMilliseconds = 10000;

            MaxConnectionID = 0;

            for (int i = 0; i < groupCount; i++)
            {
                string Id = Guid.NewGuid().ToString();

                Teams.Add(Id, new TeamGroup(Id, MaxUserPerTeam));
                TeamIdList.Add(Id);
            }

        }

        public void OnGameStart()
        {
            GameStartDate = DateTime.UtcNow;

            this.StartGameRoom();
        }
        private void StartGameRoom()
        {
            TickRate = 15;
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

        public void OnObjectMove(ShiftServerData data, ShiftClient client)
        {

        }

        public void OnObjectUse(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerCreate(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerJoin(Character chardata, ShiftClient shift)
        {
            ShiftServerData sendData = new ShiftServerData();
            string clientSessionId = shift.UserSessionID;
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();
            Player currentPlayer = (Player)gameObjectList.Where(x => x.OwnerConnectionID == shift.ConnectonID && x.GetType() == typeof(Player)).FirstOrDefault();

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
                    AttackSpeed = (float)shift.CurrentObject.AttackSpeed,
                    MovementSpeed = (float)shift.CurrentObject.MovementSpeed
                };



            }
            else
            {
                Player player = new Player();
                player.OwnerConnectionID = shift.ConnectonID;
                player.OwnerSessionID = shift.UserSessionID;
                player.ObjectID = Interlocked.Increment(ref ObjectCounter);
                player.Name = chardata.Name;
                player.MaxHP = chardata.Stats.Health;
                player.CurrentHP = chardata.Stats.Health;
                player.AttackSpeed = chardata.Stats.AttackSpeed;
                player.MovementSpeed = chardata.Stats.MovementSpeed;
                player.CurrentHP = chardata.Stats.Health;
                player.Position = new Vector3(0.0f, 0.0f, 0.0f);
                player.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
                player.Scale = new Vector3(1f, 1f, 1f);

                this.OnPlayerCreate(player);
                log.Info($"[CreatePlayer] OnRoom:{this.ID} Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.ConnectonID}");

              
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
            shift.SendPacket(MSPlayerEvent.CreatePlayer, sendData);
        }

        public void BroadcastToRoom(ShiftClient currentClient, MSServerEvent evt)
        {
            RoomPlayerInfo pInfo = new RoomPlayerInfo();
            pInfo.Username = currentClient.UserName;

            if (currentClient.JoinedTeamID != null)
                pInfo.TeamId = currentClient.JoinedTeamID;

            pInfo.IsJoinedToTeam = currentClient.IsJoinedToTeam;
            pInfo.IsReady = currentClient.IsReady;
            if (currentClient.CurrentObject != null)
            {
                pInfo.ObjectId = currentClient.CurrentObject.ObjectID;
                pInfo.CurrentGObject = new PlayerObject
                {
                    Name = currentClient.UserName,
                    Oid = currentClient.CurrentObject.ObjectID,
                    AttackSpeed = (float)currentClient.CurrentObject.AttackSpeed,
                    MovementSpeed = (float)currentClient.CurrentObject.MovementSpeed,
                    CurrentHp = currentClient.CurrentObject.CurrentHP,
                    MaxHp = currentClient.CurrentObject.MaxHP,
                    PosX = currentClient.CurrentObject.Position.X,
                    PosY = currentClient.CurrentObject.Position.Y,
                    PosZ = currentClient.CurrentObject.Position.Z
                };
            }

            if (this.RoomLeaderID == currentClient.ConnectonID)
            {
                pInfo.IsLeader = true;
            }
            else
            {
                pInfo.IsLeader = false;
            }

            ShiftServerData data = new ShiftServerData();
            data.RoomData = new RoomData();
            data.RoomData.PlayerInfo = pInfo;

            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                if (clientList[i].ConnectonID == currentClient.ConnectonID)
                    continue;

                clientList[i].SendPacket(evt, data);
            }
        }
        public void BroadcastDataToRoom(ShiftClient currentClient, MSServerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                if (clientList[i].ConnectonID == currentClient.ConnectonID)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }

        public void BroadcastDataToRoom(ShiftClient currentClient, MSPlayerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                if (clientList[i].ConnectonID == currentClient.ConnectonID)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }
        public void SendRoomState(TimeSpan timespan)
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

                data.GoUpdatePacket.PlayerList.Add(new PlayerObject
                {
                    Oid = gObject.ObjectID,
                    PosX = gObject.Position.X,
                    PosY = gObject.Position.Y,
                    PosZ = gObject.Position.Z,
                    MovementSpeed = (float)gObject.MovementSpeed,
                    AttackSpeed = (float)gObject.AttackSpeed,
                    CurrentHp = gObject.CurrentHP,
                    LastProcessedSequenceID = gObject.LastProcessedSequenceID
                });
            }

            List<ShiftClient> clientList = Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                clientList[i].SendPacket(MSPlayerEvent.RoomUpdate, data);
            }
        }
        public void OnRoomUpdate()
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

                IGameInput gInput = null;
                PlayerInput pInput = null;
                for (int kk = 0; kk <= gObject.GameInputs.Count; kk++)
                {
                    gObject.GameInputs.TryDequeue(out gInput);
                    if (gInput != null)
                    {
                        switch (gInput.EventType)
                        {
                            case MSPlayerEvent.Move:
                                
                                gObject.OnMove(gInput.Vector);
                                log.Debug($"gObject: {gObject.ObjectID} Move  {gInput.Vector.ToString()}!");
                                break;
                            case MSPlayerEvent.Attack:
                                break;
                            case MSPlayerEvent.Dead:
                                break;
                            case MSPlayerEvent.Use:
                                break;
                            default:
                                break;
                        }

                        gObject.LastProcessedSequenceID = gInput.SequenceID;
                    }
                    //pInput = (PlayerInput)gInput;
                }

            }

            SendRoomState(updatePassTime);
        }


        public IGroup GetRandomTeam()
        {
            var RoomTeams = this.Teams.GetValues();

            for (int i = 0; i < RoomTeams.Count; i++)
            {
                if (RoomTeams[i].MaxPlayer > RoomTeams[i].Clients.Count)
                {
                    if (LastActiveTeam != null)
                    {
                        if (LastActiveTeam == RoomTeams[i].ID)
                        {
                            IGroup otherTeam = null;
                            var otherTeamId = TeamIdList.Where(x => x != RoomTeams[i].ID).FirstOrDefault();
                            Teams.TryGetValue(otherTeamId, out otherTeam);
                            if (otherTeam != null)
                            {
                                if (otherTeam.MaxPlayer > otherTeam.Clients.Count)
                                {
                                    LastActiveTeam = otherTeam.ID;
                                    return otherTeam;
                                }
                                else
                                    return null;

                            }
                            else
                            {
                                LastActiveTeam = RoomTeams[i].ID;
                                return RoomTeams[i];
                            }
                        }
                    }
                    else
                    {
                        LastActiveTeam = RoomTeams[i].ID;
                        return RoomTeams[i];
                    }

                }
            }

            return null;
        }
        public ShiftClient SetRandomNewLeader()
        {
            ShiftClient shift = null;
            var AllClients = this.Clients.GetValues();
            foreach (var item in AllClients)
            {
                this.Clients.TryGetValue(item.ConnectonID, out shift);

                if (shift != null && this.RoomLeaderID == -1)
                {

                    if (this.RoomLeaderID != shift.ConnectonID)
                    {
                        this.RoomLeaderID = shift.ConnectonID;
                        shift.IsJoinedToRoom = true;
                        shift.JoinedRoomID = this.ID;
                        return shift;
                    }

                }
            }


            return null;
        }

        public void OnObjectCreate(IGameObject gameObject)
        {
        }
    }
}
