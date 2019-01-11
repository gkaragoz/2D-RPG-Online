using Google.Protobuf;
using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Auth
{
    public class ShiftClient : IClient
    {
        private static readonly log4net.ILog log
                = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Session UserSession { get; set; }
        public TcpClient Client { get; set; }
        public string UserName { get; set; }
        public int connectionId { get; set; }
        public bool IsJoinedToWorld { get; set; }
        public bool IsReady { get; set; }
        public bool IsJoinedToRoom { get; set; }
        public bool IsJoinedToTeam { get; set; }
        public string JoinedRoomId { get; set; }
        public string JoinedTeamId { get; set; }
        public bool IsConnected()
        {
            return Client.Connected;
        }

        public bool SendPacket(MSServerEvent eventType, ShiftServerData data)
        {
            try
            {
                data.Basevtid = MSBaseEventId.ServerEvent;
                data.Svevtid = eventType;
                byte[] bb = data.ToByteArray();
                return Send(bb);
            }
            catch (Exception err)
            {
                log.Error($"[SendPacket] Remote:{this.Client.Client.RemoteEndPoint.ToString()} ClientNo:{this.connectionId}", err);
                return false;
            }
                 
        }
        public bool SendPacket(MSPlayerEvent eventType, ShiftServerData data)
        {
            try
            {
                data.Basevtid = MSBaseEventId.PlayerEvent;
                data.Plevtid = eventType;
                byte[] bb = data.ToByteArray();
                return Send(bb);
            }
            catch (Exception err)
            {
                log.Error($"[SendPacket] Remote:{this.Client.Client.RemoteEndPoint.ToString()} ClientNo:{this.connectionId}", err);
                return false;
            }

        }
        public IGroup GetJoinedTeam(IRoom room)
        {

            IGroup group = null;
            if (this.IsJoinedToTeam)
            {
                room.Teams.TryGetValue(this.JoinedTeamId, out group);

                if (group != null)
                {
                    return group;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public bool JoinTeam(IGroup group)
        {

            if (group != null)
            {
                if (this.IsJoinedToTeam)
                {
                    log.Info($"[JoinTeam] Remote:{this.Client.Client.RemoteEndPoint.ToString()} ClientNo:{this.connectionId} Already in a team");
                    return false;
                }
                else
                {
                    group.AddPlayer(this);
                    return true;
                }
            }
            else
                return false;
       
        }
        public bool LeaveFromTeam(IRoom room)
        {

            IGroup group = null;
            if (this.IsJoinedToTeam)
            {
                room.Teams.TryGetValue(this.JoinedTeamId, out group);

                if (group != null)
                {
                    group.RemovePlayer(this);
                    this.IsJoinedToTeam = false;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private bool Send(byte[] bb)
        {
            if (this.Client != null)
            {
                try
                {
                    NetworkStream stream = Client.GetStream();
                    return SendMessage(stream, bb);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Server.SendMessage exception: " + exception);
                    return false;
                }
            }
            else
                return false;
        }
        // send message (via stream) with the <size,content> message structure
        protected static bool SendMessage(NetworkStream stream, byte[] content)
        {
            // can we still write to this socket (not disconnected?)
            if (!stream.CanWrite)
            {
                return false;
            }

            // stream.Write throws exceptions if client sends with high
            // frequency and the server stops
            try
            {
                // construct header (size)
                byte[] header = ShiftHelper.IntToBytes(content.Length);

                // write header+content at once via payload array. writing
                // header,payload separately would cause 2 TCP packets to be
                // sent if nagle's algorithm is disabled(2x TCP header overhead)
                byte[] payload = new byte[header.Length + content.Length];
                Array.Copy(header, payload, header.Length);
                Array.Copy(content, 0, payload, header.Length, content.Length);
                stream.Write(payload, 0, payload.Length);

                return true;
            }
            catch (Exception exception)
            {
                // log as regular message because servers do shut down sometimes
                return false;
            }
        }

    }

    public static class ShiftHelper
    {
        public static ShiftClient GetShiftClient(List<ShiftClient> shifts, TcpClient tcpclient)
        { 
            return shifts.Where(x => x.Client == tcpclient).FirstOrDefault();
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
