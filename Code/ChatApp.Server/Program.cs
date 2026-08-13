using System.Net;
using ChatApp.Server.Network;

namespace ChatApp.Server
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Tcp ChatApp Server";
            Console.WriteLine("Starting Server...");

            var server = new TcpServerListener(IPAddress.Parse("127.0.0.1"), 5000);
            Console.CancelKeyPress += (_, _) => server.Stop();

            await server.StartAsync();
        }
    }
}