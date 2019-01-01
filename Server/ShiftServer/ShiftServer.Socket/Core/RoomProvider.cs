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

        private static IRoom room = null;
        public ServerDataHandler dataHandler = null;


        public RoomProvider(IRoom room) { }
    }
}
