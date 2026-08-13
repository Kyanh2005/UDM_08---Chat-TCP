using System.Net.Sockets;

namespace ChatApp.Server.Network
{
    public class ClientSession
    {
        public string Username { get; set; } = string.Empty;
        public TcpClient ClientSocket { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.Now;

        public ClientSession(TcpClient clientSocket)
        {
            ClientSocket = clientSocket;
        }
    }
}