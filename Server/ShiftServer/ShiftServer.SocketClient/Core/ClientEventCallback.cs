using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Client.Core
{
    public class ClientEventCallback
    {
        public MSServerEvent EventId { get; set; }
        public Action<ShiftServerData> CallbackFunc { get; set; }

    }

    public class PlayerEventCallback
    {
        public MSPlayerEvent EventId { get; set; }
        public Action<ShiftServerData> CallbackFunc { get; set; }

    }

    public static class ClientEventInvoker
    {

        /// <summary>
        /// Invoke Player Events
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<PlayerEventCallback> events, ShiftServerData data)
        {

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.Plevtid)
                {
                    events[i].CallbackFunc.Invoke(data);
                }


            }

        }

        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<ClientEventCallback> events, ShiftServerData data)
        {

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.Svevtid)
                {
                    events[i].CallbackFunc.Invoke(data);
                }


            }

        }
    }
}
