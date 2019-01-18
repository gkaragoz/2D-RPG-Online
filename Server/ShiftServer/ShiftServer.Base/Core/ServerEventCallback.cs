﻿using ShiftServer.Base.Auth;
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
        public Action<ShiftServerData, ShiftClient> CallbackFunc { get; set; }

    }

    public class PlayerEventCallback
    {
        public MSPlayerEvent EventID { get; set; }
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
        public static void Fire(List<PlayerEventCallback> events, ShiftServerData data, ShiftClient shift)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventID == data.Plevtid)
                    {
                        events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {

                if (shift.Client != null)
                {
                    if (shift.Client.Client.Connected)
                        ServerProvider.log.Error($"[EXCEPTION] ClientNO: {shift.ConnectonID} REMOTE: " + shift.Client.Client.RemoteEndPoint.ToString(), err);
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
        public static void Fire(List<ServerEventCallback> events, ShiftServerData data, ShiftClient shift)
        {
            try
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].EventID == data.Svevtid)
                    {
                        events[i].CallbackFunc.Invoke(data, shift);
                    }


                }
            }
            catch (Exception err)
            {
                if (shift.Client != null)
                {
                    if (shift.Client.Client.Connected)
                        ServerProvider.log.Error($"[EXCEPTION] ClientNO: {shift.ConnectonID} REMOTE: " + shift.Client.Client.RemoteEndPoint.ToString(), err);
                }
                return;
            }

        }
    }
}
