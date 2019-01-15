using ShiftServer.Base.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class ServerEventCallback
    {
        public MSServerEvent EventId { get; set; }
        public Action<ShiftServerData, ShiftClient> CallbackFunc { get; set; }

    }

    public class PlayerEventCallback
    {
        public MSPlayerEvent EventId { get; set; }
        public Action<ShiftServerData, ShiftClient> CallbackFunc { get; set; }

    }
    public static class ServerEventInvoker
    {

        /// <summary>
        /// Invoke Player Events
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<PlayerEventCallback> events, ShiftServerData data, ShiftClient shift, log4net.ILog log)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventId == data.Plevtid)
                    {
                        events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {

                log.Error($"[EXCEPTION] ClientNO: {shift.connectionId} REMOTE: " + shift.Client.Client.RemoteEndPoint.ToString(), err);
                return;
            }
          

        }

        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static void Fire(List<ServerEventCallback> events, ShiftServerData data, ShiftClient shift, log4net.ILog log)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventId == data.Svevtid)
                    {
                        events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {
                if (shift.Client.Client.Connected)
                    log.Error($"[EXCEPTION] ClientNO: {shift.connectionId} REMOTE: " + shift.Client.Client.RemoteEndPoint.ToString(), err);
                return;
            }

        }
    }
}
