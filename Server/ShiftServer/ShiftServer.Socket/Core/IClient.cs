using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    public interface IClient
    {
        bool IsConnected();
        int connectionId { get; set; }
        Session UserSession { get; set; }
        TcpClient Client { get; set; }

    }
}
