using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ChatApp.Server.Services;

namespace ChatApp.Server.Network
{
    public class TcpServerListener
    {
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<string, ClientSession> _sessions = new();
        private bool _isRunning;

        public TcpServerListener(IPAddress ipAddress, int port)
        {
            _listener = new TcpListener(ipAddress, port);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;
            ServerLogger.Log($"Server started on {_listener.LocalEndpoint}");

            try
            {
                while (_isRunning)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (ObjectDisposedException) when (!_isRunning)
            {
                ServerLogger.Log("Server stopped.");
            }
            catch (SocketException) when (!_isRunning)
            {
                ServerLogger.Log("Server socket closed.");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
            var session = new ClientSession(client);
            _sessions[endpoint] = session;

            ServerLogger.Log($"Client connected from: {endpoint}");

            try
            {
                await Task.Delay(1000);
            }
            finally
            {
                _sessions.TryRemove(endpoint, out _);
                client.Dispose();
            }
        }
    }
}