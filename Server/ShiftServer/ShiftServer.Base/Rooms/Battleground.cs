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
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime GameStartDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsStopTriggered { get; set; }
        public int CreatedUserId { get; set; }
        public int ServerLeaderId { get; set; }
        public int DisposeInMilliseconds { get; set; }
        public int MaxConnId { get; set; }
        public int MaxUserPerTeam { get; set; }
        public SafeDictionary<string, IGroup> Teams { get; set; }
        public List<string> TeamIdList { get; set; }
        public string LastActiveTeam { get; set; }
        public DBServiceProvider _ctx { get; set; }

        public int ObjectCounter = 0;
        public int PlayerCounter = 0;

        public UpdateGOList GOUpdatePacket = new UpdateGOList();


        public Battleground(int groupCount, int maxUserPerTeam, DBServiceProvider ctx)
        {
            _ctx = ctx;
            MaxUserPerTeam = maxUserPerTeam;
            Clients = new SafeDictionary<int, ShiftClient>();
            SocketIdSessionLookup = new SafeDictionary<string, int>();
            GameObjects = new SafeDictionary<int, IGameObject>();
            Teams = new SafeDictionary<string, IGroup>();
            TeamIdList = new List<string>();

            Id = "123";
            DisposeInMilliseconds = 10000;

            MaxConnId = 0;

            for (int i = 0; i < groupCount; i++)
            {
                string Id = Guid.NewGuid().ToString();

                Teams.Add(Id, new TeamGroup(Id, MaxUserPerTeam));
                TeamIdList.Add(Id);
            }

        }

        public void OnGameStart()
        {
            this.StartGameRoom();
        }
        private void StartGameRoom()
        {
            int timerInterval = TickrateUtil.Set(10);


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
            gameObject.ObjectId = Interlocked.Increment(ref ObjectCounter);
            GameObjects.Add(gameObject.ObjectId, gameObject);
            GOUpdatePacket.PlayerList.Add(new PlayerObject
            {
                Oid = gameObject.ObjectId,
                PosX = gameObject.Position.X,
                PosY = gameObject.Position.Y,
                PosZ = gameObject.Position.Z,
            });
        }

        public void OnObjectDestroy(IGameObject gameObject)
        {
            GameObjects.Remove(gameObject.ObjectId);
            var item = GOUpdatePacket.PlayerList.Where(x => x.Oid == gameObject.ObjectId).FirstOrDefault();
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
            string clientSessionId = shift.UserSession.GetSid();
            if (clientSessionId == null)
                return;

            List<IGameObject> gameObjectList = GameObjects.GetValues();
            Player currentPlayer = (Player)gameObjectList.Where(x => x.OwnerConnectionId == shift.connectionId && x.GetType() == typeof(Player)).FirstOrDefault();

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
                player.OwnerConnectionId = shift.connectionId;
                player.OwnerSessionId = shift.UserSession.GetSid();
                player.ObjectId = Interlocked.Increment(ref ObjectCounter);
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
                log.Info($"[CreatePlayer] OnRoom:{this.Id} Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

                sendData.SPlayerObject = new PlayerObject
                {
                    CurrentHp = player.CurrentHP,
                    MaxHp = player.MaxHP,
                    Oid = player.ObjectId,
                    AttackSpeed = (float)player.AttackSpeed,
                    MovementSpeed = (float)player.MovementSpeed,
                    PosX = player.Position.X,
                    PosY = player.Position.Y,
                    PosZ = player.Position.Z
                };


                currentPlayer = player;
            }
            shift.CurrentObject = currentPlayer;
            shift.Inputs = new SafeQueue<IGameInput>();
            shift.SendPacket(MSPlayerEvent.CreatePlayer, sendData);
        }

        public void BroadcastToRoom(ShiftClient currentClient, MSServerEvent evt)
        {
            RoomPlayerInfo pInfo = new RoomPlayerInfo();
            pInfo.Username = currentClient.UserName;

            if (currentClient.JoinedTeamId != null)
                pInfo.TeamId = currentClient.JoinedTeamId;

            pInfo.IsJoinedToTeam = currentClient.IsJoinedToTeam;
            pInfo.IsReady = currentClient.IsReady;

            if (this.ServerLeaderId == currentClient.connectionId)
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
                if (clientList[i].UserSession == null)
                    continue;

                if (clientList[i].connectionId == currentClient.connectionId)
                    continue;

                clientList[i].SendPacket(evt, data);
            }
        }
        public void BroadcastDataToRoom(ShiftClient currentClient, MSServerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                if (clientList[i].connectionId == currentClient.connectionId)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }

        public void BroadcastDataToRoom(ShiftClient currentClient, MSPlayerEvent state, ShiftServerData data)
        {
            List<ShiftClient> clientList = this.Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                if (clientList[i].connectionId == currentClient.connectionId)
                    continue;

                clientList[i].SendPacket(state, data);
            }
        }
        public void SendRoomState()
        {

            ShiftServerData data = new ShiftServerData();
            data.GoUpdatePacket = new UpdateGOList();

            IGameObject gObject = null;
            for (int i = 0; i <= ObjectCounter; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                if (gObject == null)
                    continue;

                data.GoUpdatePacket.PlayerList.Add(new PlayerObject
                {
                    Oid = gObject.ObjectId,
                    PosX = (float)gObject.Position.X,
                    PosY = (float)gObject.Position.Y,
                    PosZ = (float)gObject.Position.Z,
                    MovementSpeed = (float)gObject.MovementSpeed,
                    AttackSpeed = (float)gObject.AttackSpeed,
                    CurrentHp = gObject.CurrentHP
                });
            }

            List<ShiftClient> clientList = Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSession == null)
                    continue;

                clientList[i].SendPacket(MSPlayerEvent.RoomUpdate, data);
            }
        }
        public void OnRoomUpdate()
        {

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
                        switch (gInput.eventType)
                        {
                            case MSPlayerEvent.Move:
                                gObject.OnMove(gInput.vector3);
                                log.Debug($"gObject: {gObject.ObjectId} Move  {gInput.vector3.ToString()}!");

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
                    }
                    //pInput = (PlayerInput)gInput;
                }

            }

            SendRoomState();
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
                        if (LastActiveTeam == RoomTeams[i].Id)
                        {
                            IGroup otherTeam = null;
                            var otherTeamId = TeamIdList.Where(x => x != RoomTeams[i].Id).FirstOrDefault();
                            Teams.TryGetValue(otherTeamId, out otherTeam);
                            if (otherTeam != null)
                            {
                                if (otherTeam.MaxPlayer > otherTeam.Clients.Count)
                                {
                                    LastActiveTeam = otherTeam.Id;
                                    return otherTeam;
                                }
                                else
                                    return null;

                            }
                            else
                            {
                                LastActiveTeam = RoomTeams[i].Id;
                                return RoomTeams[i];
                            }
                        }
                    }
                    else
                    {
                        LastActiveTeam = RoomTeams[i].Id;
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
                this.Clients.TryGetValue(item.connectionId, out shift);

                if (shift != null && this.ServerLeaderId == -1)
                {

                    if (this.ServerLeaderId != shift.connectionId)
                    {
                        this.ServerLeaderId = shift.connectionId;
                        shift.IsJoinedToRoom = true;
                        shift.JoinedRoomId = this.Id;
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
