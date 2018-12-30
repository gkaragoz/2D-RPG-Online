using ShiftServer.Server.Core;
using System;


namespace ShiftServer.Server.Factory.Movement
{
    public static class MotionMaster
    {
        public static void OnMove(ShiftServerData data)
        {
            Console.WriteLine("OnMove event triggered::event_id::" + (int)data.Plevtid);
        }
    }
}
