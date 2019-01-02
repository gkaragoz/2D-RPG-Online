using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    public class RoomProvider
    {
        private static readonly log4net.ILog log
                  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ServerDataHandler dataHandler = null;
        private ServerProvider _sp = null;

        public RoomProvider(ServerProvider mainServerProvider) {
            _sp = mainServerProvider;
        }
        public void CreateRoom(IRoom room)
        {
            room.Guid = Guid.NewGuid().ToString();
            _sp.world.Rooms.Add(room.Guid, room);
        }

        public void OnRoomCreate(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomCreate");

            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomCreate, data);
        }
        public void OnRoomJoin(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomJoin");

            _sp.SendMessage(shift.connectionId, MSServerEvent.RoomJoin, data);
        }

        public void OnRoomGameStart(ShiftServerData data, ShiftClient shift)
        {
            log.Info($"ClientNO: {shift.connectionId} ------> RoomGameStart");
            _sp.SendMessage(shift.connectionId, MSServerEvent.PingRequest, data);
        }

    }
}
