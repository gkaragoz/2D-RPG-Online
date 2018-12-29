using ShiftServer.Proto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Handlers
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class DataHandler
    {
        public List<EventCallback> events = null;

        public DataHandler()
        {
            events = new List<EventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);


            Event.Fire(events, data);
        }
    }
}
