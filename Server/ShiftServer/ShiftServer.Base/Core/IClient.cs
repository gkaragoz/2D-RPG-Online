using System.Net.Sockets;


namespace ShiftServer.Base.Core
{
    public interface IClient
    {
        bool IsConnected();
        int connectionId { get; set; }
        Session UserSession { get; set; }
        TcpClient Client { get; set; }

    }
}
