using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class MsgManager
    {
        public List<EventCallback> events = null;

        public MsgManager() {
            events = new List<EventCallback>();
        }

        private static ShiftServerMsg data = null;

        public void HandleMessage(byte[] bb)
        {
            data = ShiftServerMsg.Parser.ParseFrom(bb);


            switch (data.MsgId)
            {
                case ShiftServerMsgID.ShiftServerInvalid:
                    break;
                case ShiftServerMsgID.ShiftServerRouterPingRequest:
                    break;
                case ShiftServerMsgID.ShiftServerRouterPingReply:
                    break;
                case ShiftServerMsgID.ShiftServerGameserverPingRequest:
                    break;
                case ShiftServerMsgID.ShiftServerGameserverPingReply:
                    break;
                case ShiftServerMsgID.ShiftServerGameserverSessionRequest:
                    break;  
                case ShiftServerMsgID.ShiftServerGameserverSessionEstablished:
                    break;
                case ShiftServerMsgID.ShiftServerNoSession:
                    break;
                case ShiftServerMsgID.ShiftServerDiagnostic:
                    break;
                case ShiftServerMsgID.ShiftServerJoinRequest:
                    break;
                case ShiftServerMsgID.ShiftServerStats:
                    break;
                case ShiftServerMsgID.ShiftServerConnectRequest:
                    break;
                case ShiftServerMsgID.ShiftServerConnectOk:
                    break;
                case ShiftServerMsgID.ShiftServerConnectionClosed:
                    break;
                case ShiftServerMsgID.ShiftServerNoConnection:
                    break;
                default:
                    break;
            }

            Event.Fire(events, data);
        }
    }
}
