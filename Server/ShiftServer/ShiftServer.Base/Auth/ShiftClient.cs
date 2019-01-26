using Google.Protobuf;
using ShiftServer.Base.Core;
using ShiftServer.Proto.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Auth
{
    public class ShiftClient : ManaSocket
    {
        private static readonly log4net.ILog log
                = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string UserSessionID { get; set; }
        public TcpClient TCPClient { get; set; }
        public IGameObject CurrentObject { get; set; }
        public SafeQueue<IGameInput> Inputs { get; set; }
        public string UserName { get; set; }
        public int ConnectionID { get; set; }
        public bool IsJoinedToWorld { get; set; }
        public bool IsReady { get; set; }
        public bool IsJoinedToRoom { get; set; }
        public bool IsJoinedToTeam { get; set; }
        public string JoinedRoomID { get; set; }
        public string JoinedTeamID { get; set; }
        public bool IsConnected { get => this.TCPClient.Connected; }

        public async Task SendPacket(MSServerEvent eventType, ShiftServerData data)
        {
            try
            {
                data.Basevtid = MSBaseEventId.ServerEvent;
                data.Svevtid = eventType;
                byte[] bb = data.ToByteArray();
                await Task.Run(() => ServerProvider.instance.server.Send(ConnectionID, bb));
            }
            catch (Exception err)
            {
                if (this.TCPClient != null)
                {
                    if (this.TCPClient.Connected)
                        log.Error($"[SendPacket] Remote:{this.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{this.ConnectionID}", err);
                }
            }

        }
        public async Task SendPacketAsync(MSPlayerEvent eventType, ShiftServerData data)
        {
            try
            {
                data.Basevtid = MSBaseEventId.PlayerEvent;
                data.Plevtid = eventType;
                byte[] bb = data.ToByteArray();
                await Task.Run(() => ServerProvider.instance.server.Send(ConnectionID, bb));
            }
            catch (Exception err)
            {
                if (this.TCPClient.Connected)
                    log.Error($"[SendPacket] Remote:{this.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{this.ConnectionID}", err);
            }

        }
        public void SendPacket(MSPlayerEvent eventType, ShiftServerData data)
        {
            try
            {
                data.Basevtid = MSBaseEventId.PlayerEvent;
                data.Plevtid = eventType;
                byte[] bb = data.ToByteArray();
                ServerProvider.instance.server.Send(ConnectionID, bb);
            }
            catch (Exception err)
            {
                if (this.TCPClient.Connected)
                    log.Error($"[SendPacket] Remote:{this.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{this.ConnectionID}", err);
            }

        }
        public IGroup GetJoinedTeam(IRoom room)
        {

            IGroup group = null;
            if (!this.IsJoinedToTeam)
                return null;

            room.Teams.TryGetValue(this.JoinedTeamID, out group);

            if (group != null)
                return group;
            else
                return null;

        }
        public bool JoinTeam(IGroup group)
        {
            if (group == null)
                return false;

            if (this.IsJoinedToTeam)
            {
                log.Info($"[JoinTeam] Remote:{this.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{this.ConnectionID} Already in a team");
                return false;
            }
            else
            {
                group.AddPlayer(this);
                return true;
            }


        }
        public bool LeaveFromTeam(IRoom room)
        {
            IGroup group = null;
            if (!this.IsJoinedToTeam)
                return false;

            room.Teams.TryGetValue(this.JoinedTeamID, out group);
            if (group == null)
                return false;

            group.RemovePlayer(this);
            this.IsJoinedToTeam = false;
            return true;
        }
        private void HardDisconnect()
        {
            if (this.TCPClient.Connected)
            {
                this.TCPClient.Dispose();
                this.TCPClient.Close();
            }
        }
        public void Dispose()
        {
            IRoom room = null;
            string userSessionId = this.UserSessionID;

            if (this == null)
                return;

            if (string.IsNullOrEmpty(userSessionId))
                return;

            ServerProvider.instance.world.ClientDispose(this);
            ServerProvider.instance.world.Clients.Remove(this.ConnectionID);

            if (!string.IsNullOrEmpty(this.JoinedRoomID))
                ServerProvider.instance.world.Rooms.TryGetValue(this.JoinedRoomID, out room);

            if (room != null)
            {

                this.IsJoinedToRoom = false;
                this.JoinedRoomID = null;
                bool isDestroyed = false;

                room.Clients.Remove(this.ConnectionID);

                if (room.Clients.Count == 0 && !room.IsPersistence)
                {
                    ServerProvider.instance.world.Rooms.Remove(room.ID);
                    isDestroyed = true;
                }
                else
                {
                    if (this.CurrentObject != null)
                        room.GameObjects.Remove(this.CurrentObject.ObjectID);
                }

                if (!isDestroyed)
                    room.BroadcastClientState(this, MSServerEvent.RoomPlayerLeft);
                else
                    RoomProvider.instance.OnRoomDispose(room);

            }

            IGroup group = null;
            if (!string.IsNullOrEmpty(this.JoinedTeamID))
                room.Teams.TryGetValue(this.JoinedTeamID, out group);

            if (group != null)
                group.RemovePlayer(this);

            HardDisconnect();
        }
        public async Task<bool> SessionCheckAsync(ShiftServerData data)
        {
            bool result = false;
            //session check
            if (data.SessionID == null)
                return result;

            AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);

            if (session == null)
                return result;

            this.UserSessionID = data.SessionID;
            Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);
            if (acc == null)
                return result;

            //check if already logged in
            List<ShiftClient> clients = ServerProvider.instance.world.Clients.GetValues();
            var client = clients.Find(x => x.UserSessionID == session.SessionID);

            if (session.SessionID == data.SessionID)
                result = true;

            if (client == null && this.IsJoinedToWorld != true)
                result = false;

            if (result)
                return result;
            else
            {
                await this.SendPacket(MSServerEvent.ConnectionFailed, new ShiftServerData { ErrorReason = ShiftServerError.BadSession });
                return result;
            }


        }
        public bool FillAccountData(ShiftServerData data)
        {
            try
            {
                AccountSession session = DBContext.ctx.Sessions.FindBySessionID(data.SessionID);
                Account acc = DBContext.ctx.Accounts.GetByUserID(session.UserID);

                this.UserSessionID = data.SessionID;


                data.Account = null;
                data.AccountData = new CommonAccountData();
                data.AccountData.VirtualMoney = acc.Gold;
                data.AccountData.VirtualSpecialMoney = acc.Gem;

                if (string.IsNullOrEmpty(acc.SelectedCharName))
                    this.UserName = data.AccountData.Username;
                else
                {
                    data.AccountData.Username = acc.SelectedCharName;
                    this.UserName = acc.SelectedCharName;
                }


                return true;
            }
            catch (Exception err)
            {
                log.Error($"[Login Failed] Remote:{this.TCPClient.Client.RemoteEndPoint.ToString()} ClientNo:{this.ConnectionID}", err);
                return false;

            }

        }

        public NetworkIdentifier CreateNetIdentifierMessage()
        {
            PlayerObject obj = new PlayerObject
            {
                Name = this.CurrentObject.Name,
                AttackSpeed = (float)this.CurrentObject.AttackSpeed,
                MoveSpeed = (float)this.CurrentObject.MovementSpeed,
                AttackDamage = this.CurrentObject.AttackDamage,
                AttackRange = (float)this.CurrentObject.AttackRange,
                CurrentHp = this.CurrentObject.CurrentHP,
                MaxHp = this.CurrentObject.MaxHP,
                CurrentMana = this.CurrentObject.CurrentMana,
                MaxMana = this.CurrentObject.MaxMana,      
                PClass = this.CurrentObject.Class,
              
            };

            NetworkIdentifier networkIdentifier = new NetworkIdentifier
            {
                Id = this.CurrentObject.ObjectID,
                Type = NetIdentifierFlag.Player,
                PlayerObject = obj,
                RotationX = this.CurrentObject.Rotation.X,
                RotationY = this.CurrentObject.Rotation.Y,
                RotationZ = this.CurrentObject.Rotation.Z,

                PositionX = this.CurrentObject.Position.X,
                PositionY = this.CurrentObject.Position.Y,
                PositionZ = this.CurrentObject.Position.Z,

                ScaleX = this.CurrentObject.Position.X,
                ScaleY = this.CurrentObject.Position.Y,
                ScaleZ = this.CurrentObject.Position.Z,
            };

            return networkIdentifier;
        }
    }

    public static class ShiftHelper
    {
        public static ShiftClient GetShiftClient(List<ShiftClient> shifts, TcpClient tcpclient)
        {
            return shifts.Where(x => x.TCPClient == tcpclient).FirstOrDefault();
        }

        // static helper functions /////////////////////////////////////////////
        // fast int to byte[] conversion and vice versa
        // -> test with 100k conversions:
        //    BitConverter.GetBytes(ushort): 144ms
        //    bit shifting: 11ms
        // -> 10x speed improvement makes this optimization actually worth it
        // -> this way we don't need to allocate BinaryWriter/Reader either
        // -> 4 bytes because some people may want to send messages larger than
        //    64K bytes
        public static byte[] IntToBytes(int value)
        {
            return new byte[] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            };
        }

        public static int BytesToInt(byte[] bytes)
        {
            return
                bytes[0] |
                (bytes[1] << 8) |
                (bytes[2] << 16) |
                (bytes[3] << 24);

        }
    }


}
