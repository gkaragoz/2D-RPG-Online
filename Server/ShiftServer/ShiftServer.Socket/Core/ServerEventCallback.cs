using ShiftServer.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    public class ServerEventCallback
    {
        public ServerEventId EventId { get; set; }
        public Action<ShiftServerData, ShiftClient> CallbackFunc { get; set; }

    }
    public static class ServerEventInvoker
    {

        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<ServerEventCallback> events, ShiftServerData data, ShiftClient client)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.Eid)
                {
                    events[i].CallbackFunc.Invoke(data, client);
                }

            }
        }


    }
}
