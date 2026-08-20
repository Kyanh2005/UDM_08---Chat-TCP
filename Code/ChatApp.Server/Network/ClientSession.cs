using System.Net.Sockets;
using ChatApp.Shared.Models;
using ChatApp.Shared.Packets;

namespace ChatApp.Server.Network
{
    // ========================================================================================
    // FILE: ClientSession.cs - QUẢN LÝ PHIÊN KẾT NỐI CỦA 1 CLIENT (MEMBER 2)
    // 
    // - VAI TRÒ: Đại diện cho 1 người dùng đang kết nối vào Server.
    // - KẾT NỐI VỚI:
    //   + Member 1 (Shared): Sử dụng ChatMessage để nhận/gửi và PacketSerializer để đóng gói.
    //   + Member 2 (MessageRouter): MessageRouter giữ danh sách các ClientSession này trong Dictionary.
    //   + Member 3 (Client Network): Trực tiếp giao tiếp qua NetworkStream với TcpClient bên phía máy Client.
    //   + Đa luồng (Thread-Safety): Dùng SemaphoreSlim để khi nhiều người cùng chat, stream không bị đè/hỏng gói tin.
    // ========================================================================================
    public class ClientSession : IDisposable
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set;} = string.Empty;
        public string? AvatarBase64 {get; set;}
        public TcpClient ClientSocket { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.Now;
        public string RemoteEndPoint {get;}
        public NetworkStream Stream {get;}

        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private bool _isDisposed;

        public ClientSession(TcpClient clientSocket)
        {
            ClientSocket = clientSocket;
            RemoteEndPoint = clientSocket.Client.RemoteEndPoint?.ToString() ?? "Unknown";
            Stream = clientSocket.GetStream();
        }

        // ====================================================================================
        // HÀM: Gửi đối tượng ChatMessage tới Client
        // - Hoạt động: Chuyển ChatMessage -> Mảng Byte (có gắn 4-byte độ dài Header) -> Bắn qua Socket.
        // ====================================================================================
        public async Task<bool> SendMessageAsync(ChatMessage massage)
        {
            if (_isDisposed || !ClientSocket.Connected) return false;

            try
            {
                byte[] packet = PacketSerializer.Serialize(massage);
                return await SendRawBytesAsync(packet);

            }
            catch
            {
                return false;
            }
            
        }

        // ====================================================================================
        // HÀM: Gửi mảng Byte thô qua NetworkStream (Đảm bảo an toàn đa luồng bằng SendLock)
        // ====================================================================================
        public async Task<bool> SendRawBytesAsync(byte[] data)
        {
            if (_isDisposed || !ClientSocket.Connected) return false;

            await _sendLock.WaitAsync();
            try
            {
                if (_isDisposed || !ClientSocket.Connected) return false;
                await Stream.WriteAsync(data, 0, data.Length);
                await Stream.FlushAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Close()
        {
            Dispose();
        }

        // ====================================================================================
        // HÀM: Giải phóng tài nguyên Socket và Stream khi Client ngắt kết nối (Chống rò rỉ RAM)
        // ==================================================================================== 
       public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try { Stream?.Dispose(); } catch { }
            try { ClientSocket?.Close(); } catch { }
            try { _sendLock?.Dispose(); } catch { }
        }
    }

}