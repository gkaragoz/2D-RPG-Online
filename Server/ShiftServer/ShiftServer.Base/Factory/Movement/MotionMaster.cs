using ShiftServer.Base.Core;
using System;


namespace ShiftServer.Base.Factory.Movement
{
    public static class MotionMaster
    {
        public static void OnMove(ShiftServerData data)
        {
            Console.WriteLine("OnMove event triggered::event_id::" + (int)data.Plevtid);
        }
    }
}
