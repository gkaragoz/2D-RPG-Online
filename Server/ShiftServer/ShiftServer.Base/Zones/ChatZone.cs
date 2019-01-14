using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Base.Factory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Zones
{
    public class ChatZone
    {
        private static readonly log4net.ILog log
          = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, int> SocketIdSessionLookup { get; set; }

        public int ObjectCounter = 0;

        public ChatZone()
        {
            Clients = new SafeDictionary<int, ShiftClient>();
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///
        /// data.Session;
        /// data.AccountData;
        /// </returns>
        /// <param name="data"></param>
        /// <param name="shift"></param>
        public void OnAccountLogin(ShiftServerData data, ShiftClient shift)
        {
            if (data.ClientInfo == null)
            {
                ShiftServerData errorData = new ShiftServerData();
                errorData.ErrorReason = ShiftServerError.WrongClientData;
                log.Warn($"[Failed Login] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");
                shift.SendPacket(MSServerEvent.AccountJoin, errorData);
                return;
            }


            //check account
            string accUsername = data.Account.Username;
            string accPassword = data.Account.Password;
            //QUERY TO SOMEWHERE ELSE

            //Checking the client has only one player character under control
            shift.UserSession.SetSid(data);
            string sessionId = shift.UserSession.GetSid();
            data.Session = new SessionData();
            data.Session.Sid = sessionId;
            //check login data

            data.Account = null;
            data.AccountData = new CommonAccountData();
            data.AccountData.Username = accUsername;
            data.AccountData.VirtualMoney = 100;
            data.AccountData.VirtualSpecialMoney = 100;

            shift.SendPacket(MSServerEvent.AccountJoin, data);
            shift.UserName = data.AccountData.Username;
            SocketIdSessionLookup.Add(sessionId, shift.connectionId);
            log.Info($"[Login Success] Remote:{shift.Client.Client.RemoteEndPoint.ToString()} ClientNo:{shift.connectionId}");

        }
    }
}
