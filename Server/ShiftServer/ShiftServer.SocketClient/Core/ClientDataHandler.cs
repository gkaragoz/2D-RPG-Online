using ShiftServer.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class ClientDataHandler
    {
        public List<ClientEventCallback> clientEvents = null;
        public List<PlayerEventCallback> playerEvents = null;
        public CommonAccountData accountData  { get; private set; }
        public RoomProvider roomProvider = null;

        public ClientDataHandler()
        {
            roomProvider = new RoomProvider();
            accountData = new CommonAccountData();
            clientEvents = new List<ClientEventCallback>();
            playerEvents = new List<PlayerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);

            switch (data.Basevtid)
            {
                case MSBaseEventId.PlayerEvent:
                    ClientEventInvoker.Fire(playerEvents, data);
                    break;
                case MSBaseEventId.ServerEvent:
                    switch (data.Svevtid)
                    {
                        case MSServerEvent.AccountJoin:
                            accountData = data.AccountData;
                            break;
                        case MSServerEvent.RoomJoin:
                            roomProvider.SetCurrentJoinedRoom(data);
                            roomProvider.AddOrUpdate(data);
                            break;
                        case MSServerEvent.RoomCreate:
                            roomProvider.SetCreatedRoom(data);
                            roomProvider.AddOrUpdate(data);
                            break;
                        case MSServerEvent.RoomLeave:
                            roomProvider.DisposeRoom(data);
                            break;
                        case MSServerEvent.RoomDelete:
                            roomProvider.DisposeRoom(data);
                            break;
                        case MSServerEvent.Connection:
                            break;
                        default:
                            break;
                    }

                    ClientEventInvoker.Fire(clientEvents, data);
                    break;
                default:

                    break;

            }
        }

        public void HandleServerFailure(MSServerEvent evt, ShiftServerError err)
        {
            ClientEventInvoker.FireServerFailed(clientEvents, evt, err);
        }
        public void HandleServerSuccess(MSServerEvent evt)
        {
            ClientEventInvoker.FireSuccess(clientEvents, evt);
        }
    }
}
