using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Models
{
    /// <summary>
    /// Event object
    /// </summary>
    public class EventCallback
    {
        public ServerEventId EventId { get; set; }
        public Action<ShiftServerData> CallbackFunc { get; set; }

    }
}
