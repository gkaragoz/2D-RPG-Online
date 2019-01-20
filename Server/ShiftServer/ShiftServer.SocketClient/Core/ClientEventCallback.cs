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
        public Func<ShiftServerData, Task> CallbackFunc { get; set; }

    }

    public class PlayerEventCallback
    {
        public MSPlayerEvent EventId { get; set; }
        public Func<ShiftServerData, Task> CallbackFunc { get; set; }

    }

    public static class ClientEventInvoker
    {

        /// <summary>
        /// Invoke Player Events
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static async Task FireAsync(List<PlayerEventCallback> events, ShiftServerData data)
        {

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.Plevtid)
                {
                    await events[i].CallbackFunc.Invoke(data);
                }


            }

        }


        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static async Task FireAsync(List<ClientEventCallback> events, ShiftServerData data)
        {

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == data.Svevtid)
                {
                    await events[i].CallbackFunc.Invoke(data);
                }


            }

        }

        /// <summary>
        /// Only client side self interaction between library and client
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void FireServerFailed(List<ClientEventCallback> events, MSServerEvent evt, ShiftServerError error )
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == evt)
                {
                    ShiftServerData data = new ShiftServerData();
                    data.Basevtid = MSBaseEventId.ServerEvent;
                    data.Svevtid = evt;
                    data.ErrorReason = error;
                    events[i].CallbackFunc.Invoke(data);
                }
            }
        }
        
        /// <summary>
        /// Only client side self interaction between library and client
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void FireSuccess(List<ClientEventCallback> events, MSServerEvent evt)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == evt)
                {
                    ShiftServerData data = new ShiftServerData();
                    data.Basevtid = MSBaseEventId.ServerEvent;
                    data.Svevtid = evt;
                    events[i].CallbackFunc.Invoke(data);
                }
            }
        }
        
        /// <summary>
        /// Only client side self interaction between library and client
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void FirePlayerFailed(List<PlayerEventCallback> events, MSPlayerEvent evt, ShiftServerError error)
        {

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].EventId == evt)
                {
                    ShiftServerData data = new ShiftServerData();
                    data.Basevtid = MSBaseEventId.ServerEvent;
                    data.Plevtid = evt;
                    data.ErrorReason = error;
                    events[i].CallbackFunc.Invoke(data);
                }


            }

        }
    }
}
