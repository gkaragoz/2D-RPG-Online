using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{

    /// <summary>
    /// Event object
    /// </summary>
    public class EventCallback
    {
        public ShiftServerMsgID EventId { get; set; }
        public Action<ShiftServerMsg> CallbackFunc { get; set; }

    }

    public static class Event
    {

        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<EventCallback> events, ShiftServerMsg data)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.MsgId)
                {
                    events[i].CallbackFunc.Invoke(data);
                }
                
            }
        }

    }
}
