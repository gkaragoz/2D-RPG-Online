﻿

namespace ShiftServer.Base.Core
{
    public class GroupProvider
    {
        private static readonly log4net.ILog log
                   = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private RoomProvider _rp = null;

        public GroupProvider(RoomProvider roomProvider)
        {
            _rp = roomProvider;
        }


    }
}
