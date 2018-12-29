using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Factory.Movement
{
    public class MotionMaster
    {
        public static void OnConnected(ShiftServerData data)
        {
            Console.WriteLine("OnConnected event triggered::event_id::" + (int)data.Eid);
        }
    }
}
