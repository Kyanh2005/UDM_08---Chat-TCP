using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared.Models;
using ChatApp.Shared.Packets;

namespace ChatApp.Client.Network
{
    public class TcpClientService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        // Sự kiện gửi dữ liệu hoặc trạng thái về UI Thread
        public event Action<string>? OnDataReceived;
        public event Action<ChatMessage>? OnMessageReceived;
        public event Action<bool, string>? OnConnectionStatusChanged;

        public bool IsConnected => _client != null && _client.Connected;

        public async Task ConnectAsync(string ip, int port, int timeoutMs = 5000)
        {
            if (IsConnected) return;

            try
            {
                _client = new TcpClient();

                // Kết nối với CancellationToken/Timeout
                var connectTask = _client.ConnectAsync(ip, port);
                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMs)) != connectTask)
                {
                    _client.Close();
                    throw new TimeoutException("Kết nối tới Server quá thời gian chờ.");
                }

                _stream = _client.GetStream();

                // Cấu hình ReadTimeout và WriteTimeout
                _stream.ReadTimeout = timeoutMs;
                _stream.WriteTimeout = timeoutMs;

                _cts = new CancellationTokenSource();

                OnConnectionStatusChanged?.Invoke(true, "Kết nối thành công.");

                // Khởi chạy vòng lặp đọc bất đồng bộ
                _ = Task.Run(() => ReadLoopAsync(_cts.Token));
            }
            catch (Exception ex)
            {
                Cleanup();
                OnConnectionStatusChanged?.Invoke(false, $"Lỗi kết nối: {ex.Message}");
            }
        }

        private async Task ReadLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && IsConnected && _stream != null)
                {
                    // BƯỚC 1: Đọc 4 byte Header biểu diễn độ dài Payload (chống dính/xé gói)
                    byte[]? lengthBuffer = await PacketSerializer.ReadExactBytesAsync(_stream, 4, token);
                    if (lengthBuffer == null)
                    {
                        // Server ngắt kết nối chủ động (FIN packet)
                        throw new SocketException((int)SocketError.ConnectionReset);
                    }

                    int payloadLength = BitConverter.ToInt32(lengthBuffer, 0);
                    if (payloadLength <= 0) continue;

                    // BƯỚC 2: Đọc đúng số byte dữ liệu (Payload JSON) theo như Header thông báo
                    byte[]? payloadBuffer = await PacketSerializer.ReadExactBytesAsync(_stream, payloadLength, token);
                    if (payloadBuffer == null)
                    {
                        throw new SocketException((int)SocketError.ConnectionReset);
                    }

                    string rawData = Encoding.UTF8.GetString(payloadBuffer);

                    // Đẩy dữ liệu thô ra Event
                    OnDataReceived?.Invoke(rawData);

                    // Giải mã đối tượng ChatMessage và đẩy ra Event
                    var message = PacketSerializer.Deserialize(payloadBuffer);
                    if (message != null)
                    {
                        OnMessageReceived?.Invoke(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ngắt kết nối chủ động từ phía Client
            }
            catch (Exception ex)
            {
                // Bắt lỗi kết nối đột ngột (Server sập, đứt cáp, timeout)
                OnConnectionStatusChanged?.Invoke(false, $"Mất kết nối Server: {ex.Message}");
            }
            finally
            {
                Cleanup();
            }
        }

        /// <summary>
        /// Gửi đối tượng ChatMessage có gắn 4-byte Header độ dài
        /// </summary>
        public async Task<bool> SendMessageAsync(ChatMessage message, CancellationToken ct = default)
        {
            if (!IsConnected || _stream == null) return false;

            await _sendLock.WaitAsync(ct);
            try
            {
                byte[] packet = PacketSerializer.Serialize(message);
                await _stream.WriteAsync(packet, 0, packet.Length, ct);
                await _stream.FlushAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke(false, $"Lỗi khi gửi dữ liệu: {ex.Message}");
                Cleanup();
                return false;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// Gửi chuỗi dữ liệu (JSON) kèm 4-byte Header độ dài chuẩn giao thức Server
        /// </summary>
        public async Task<bool> SendDataAsync(string rawJson, CancellationToken ct = default)
        {
            if (!IsConnected || _stream == null) return false;

            await _sendLock.WaitAsync(ct);
            try
            {
                byte[] payloadBytes = Encoding.UTF8.GetBytes(rawJson);
                byte[] lengthBytes = BitConverter.GetBytes(payloadBytes.Length);

                byte[] fullPacket = new byte[4 + payloadBytes.Length];
                Array.Copy(lengthBytes, 0, fullPacket, 0, 4);
                Array.Copy(payloadBytes, 0, fullPacket, 4, payloadBytes.Length);

                await _stream.WriteAsync(fullPacket, 0, fullPacket.Length, ct);
                await _stream.FlushAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke(false, $"Lỗi khi gửi dữ liệu: {ex.Message}");
                Cleanup();
                return false;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Disconnect()
        {
            _cts?.Cancel();
            Cleanup();
            OnConnectionStatusChanged?.Invoke(false, "Đã ngắt kết nối.");
        }

        private void Cleanup()
        {
            _stream?.Dispose();
            _client?.Dispose();
            _stream = null;
            _client = null;
        }
    }
}