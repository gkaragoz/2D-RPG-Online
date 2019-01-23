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
        public MSServerEvent EventID { get; set; }
        public Func<ShiftServerData, ShiftClient, Task> CallbackFunc { get; set; }

    }

    public class PlayerEventCallback
    {
        public MSPlayerEvent EventID { get; set; }
        public Func<ShiftServerData, ShiftClient, Task> CallbackFunc { get; set; }

    }
    public static class ServerEventInvoker
    {

        /// <summary>
        /// Invoke Player Events
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static async Task FireAsync(List<PlayerEventCallback> events, ShiftServerData data, ShiftClient shift)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventID == data.Plevtid)
                    {
                        await events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {

                if (shift.TCPClient != null)
                {
                    if (shift.TCPClient.Client.Connected)
                        ServerProvider.log.Error($"[EXCEPTION] ClientNO: {shift.ConnectionID} REMOTE: " + shift.TCPClient.Client.RemoteEndPoint.ToString(), err);
                }
                return;
            }
          

        }

        /// <summary>
        /// Invoke client callback functions
        /// </summary>
        /// <param name="events"> Registered event list from AddEventListener</param>
        /// <param name="eventId"> Shift server event msg id</param>
        /// <param name="data"> Shift server event data</param>
        public static async Task FireAsync(List<ServerEventCallback> events, ShiftServerData data, ShiftClient shift)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventID == data.Svevtid)
                    {
                        await events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {
                if (shift.TCPClient != null)
                {
                    if (shift.TCPClient.Client.Connected)
                        ServerProvider.log.Error($"[EXCEPTION] ClientNO: {shift.ConnectionID} REMOTE: " + shift.TCPClient.Client.RemoteEndPoint.ToString(), err);
                }
            }

        }
    }
}
