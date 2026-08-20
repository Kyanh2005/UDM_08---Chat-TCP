using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ChatApp.Server.Services;
using ChatApp.Shared.Models;
using ChatApp.Shared.Packets;

namespace ChatApp.Server.Network
{
    // ========================================================================================
    // FILE: TcpServerListener.cs - TRÁI TIM TCP SERVER CORE (MEMBER 2)
    // 
    // - VAI TRÒ: 
    //   1. Mở cổng mạng (TcpListener) lắng nghe các Client kết nối tới qua async/await.
    //   2. Vòng lặp đọc gói tin chống dính/xé gói (ReadExactBytesAsync).
    //   3. Gọi DataValidator kiểm tra tính hợp lệ -> Chuyển gói tin cho MessageRouter xử lý.
    //   4. Bắt lỗi mất mạng đột ngột và dọn dẹp bộ nhớ (try...finally).
    // - KẾT NỐI VỚI:
    //   + Member 1: PacketSerializer (Deserialize dữ liệu JSON).
    //   + Member 2 Services: DataValidator (kiểm tra rác/lỗi), MessageRouter (phân luồng), ServerLogger (ghi nhật ký).
    //   + Member 3: Nhận kết nối trực tiếp từ TcpClient của Client gửi lên.
    // ========================================================================================
    public class TcpServerListener
    {
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<string, ClientSession> _sessions = new();
        private readonly MessageRouter _router;
        private readonly int _maxMessageLength;
        private readonly int _maxAvatarSizeKB;
        private bool _isRunning;
        private readonly CancellationTokenSource _cts = new();

        public ConcurrentDictionary<string, ClientSession> Session => _session;
        public TcpServerListener(IPAddress ipAddress, int port, int maxMessageLength = 5000, int maxAvatarSizeKB = 50)
        {
            _listener = new TcpListener(ipAddress, port);
            _router = new MessageRouter(_sessions);
            _maxMessageLength = maxMessageLength;
            _maxAvatarSizeKB = maxAvatarSizeKB;
        }
        // ====================================================================================
        // HÀM: Bắt đầu lắng nghe Client kết nối (Bất đồng bộ - không làm đơ ứng dụng)
        // ====================================================================================
        public async Task StartAsync()

        {
            try
            {
                _listener.Start();
                _isRunning = true;
                ServerLogger.LogSuccess($"Server is listening on {_listener.LocalEndpoint}");
            }
            
            
            while (_isRunning && !_cts.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token);

                    _ = Task.Run(() => HandleClientAsync(Client, _cts.Token));
                }
            }
            catch (ObjectionCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                if (_isRunning)
                {
                ServerLogger.LogError($"AcceptClient Error: {ex.Message}");
                }
            }
        }
        finally
        {
            Stop();
        }
    }
        // ====================================================================================
        // HÀM: Vòng lặp xử lý nhận/gửi dữ liệu riêng cho từng Client
        // - Hoạt động: Đọc 4-byte độ dài Header -> Đọc Payload JSON -> Validate -> Router -> Giải phóng socket khi ngắt.
        // ====================================================================================
    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            ClientSession? session = null;
            string remoteEp = "Unknown";

            try
            {
                session = new ClientSession(client);
                remoteEp = session.RemoteEndPoint;
                ServerLogger.LogInfo($"New connection established from {remoteEp}", remoteEp);

                var stream = session.Stream;
                byte[] lengthBuffer = new byte[4];

                while (_isRunning && !token.IsCancellationRequested && client.Connected)
                {
                    // BƯỚC 1: Đọc chính xác 4 byte đầu tiên (Header độ dài gói tin - Length-prefixed framing)
                    bool hasHeader = await ReadExactBytesAsync(stream, lengthBuffer, 0, 4, token);
                    if (!hasHeader)
                    {
                        // Client đã đóng kết nối bình thường (EOF)
                        break;
                    }

                    int payloadLength = BitConverter.ToInt32(lengthBuffer, 0);

                    // Kiểm tra độ dài hợp lệ (chống hacker gửi payload âm hoặc quá 15MB gây tràn RAM)
                    if (payloadLength <= 0 || payloadLength > 15 * 1024 * 1024)
                    {
                        ServerLogger.LogWarn($"Invalid packet payload length: {payloadLength} bytes. Dropping connection.", remoteEp);
                        break;
                    }

                    // BƯỚC 2: Đọc đúng số byte dữ liệu (Payload JSON) theo như Header đã thông báo
                    byte[] payloadBuffer = new byte[payloadLength];
                    bool hasPayload = await ReadExactBytesAsync(stream, payloadBuffer, 0, payloadLength, token);
                    if (!hasPayload)
                    {
                        break;
                    }

                    // BƯỚC 3: Giải mã mảng Byte JSON thành đối tượng ChatMessage (Gọi Member 1 PacketSerializer)
                    ChatMessage? message;
                    try
                    {
                        message = PacketSerializer.Deserialize(payloadBuffer);
                    }
                    catch (Exception jsonEx)
                    {
                        ServerLogger.LogError($"JSON Deserialization failed: {jsonEx.Message}", remoteEp);
                        var parseErrPacket = new ChatMessage
                        {
                            Type = MessageType.ERROR,
                            SenderUsername = "SERVER",
                            Content = "ERROR_400_INVALID_DATA: Malformed JSON packet structure."
                        };
                        await session.SendMessageAsync(parseErrPacket);
                        continue;
                    }

                    // BƯỚC 4: Kiểm tra tính hợp lệ qua module DataValidator (Gọi Member 2 DataValidator)
                    if (!DataValidator.ValidateMessage(message, out string validationError, _maxMessageLength, _maxAvatarSizeKB))
                    {
                        ServerLogger.LogWarn($"Validation rejected: {validationError}", remoteEp);
                        var errorPacket = new ChatMessage
                        {
                            Type = MessageType.ERROR,
                            SenderUsername = "SERVER",
                            ReceiverUsername = session.Username,
                            Content = validationError
                        };
                        await session.SendMessageAsync(errorPacket);
                        continue;
                    }

                    // BƯỚC 5: Đưa gói tin hợp lệ vào MessageRouter để phân luồng (Broadcast / Unicast / ...)
                    if (message != null)
                    {
                        await _router.RouteMessageAsync(session, message);
                    }
                }
            }
            catch (SocketException sockEx)
            {
                // Bắt lỗi mất kết nối đột ngột (rút dây mạng, tắt client ngang hông)
                ServerLogger.LogWarn($"Socket error ({sockEx.SocketErrorCode}): {sockEx.Message}", remoteEp);
            }
            catch (IOException ioEx)
            {
                ServerLogger.LogWarn($"IO/Connection reset: {ioEx.Message}", remoteEp);
            }
            catch (Exception ex)
            {
                ServerLogger.LogError($"Unhandled error in client handler: {ex.Message}", remoteEp);
            }
            finally
            {
                // BƯỚC 6: DỌN DẸP BỘ NHỚ VÀ GIẢI PHÓNG TÀI NGUYÊN (Khối try...finally an toàn)
                if (session != null)
                {
                    await _router.HandleDisconnectAsync(session); // Thông báo cho mọi người biết user này đã offline
                    session.Dispose(); // Đóng socket và giải phóng RAM
                }
                else
                {
                    try { client.Close(); } catch { }
                    try { client.Dispose(); } catch { }
                }

                ServerLogger.LogInfo($"Session closed for {remoteEp}", remoteEp);
            }
        }
    // ====================================================================================
    // HÀM: Đọc chính xác N byte từ TCP Stream (Xử lý triệt để hiện tượng phân mảnh/xé gói của TCP)
    // ====================================================================================
    private static async Task<bool> ReadExactBytesAsync(NetworkStream stream, byte[] buffer, int offset, int count, CancellationToken token)
    {
        int totalBytesRead = 0;
        while (totalBytesRead < count)
        {
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(offset + totalBytesRead, count - totalBytesRead), token);
            if (bytesRead === 0)
            {
                return false; // Socket đã bị ngắt từ phía đối phương
            }
            totalBytesRead += bytesRead;
        }
        return true;
    }
    // ====================================================================================
    // HÀM: Dừng Server và đóng toàn bộ kết nối của tất cả Client
    // ====================================================================================
    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;

        try
        {
            _cts.Cancel();
            _listener.Stop();

            foreach (var session in _sessions.Values)
            {
                session.Dispose();
            }
            _sessions.Clear();

            ServerLogger.LogWarn("Server has stopped.");
        }
        catch (Exception ex)
        {
            ServerLogger.LogError($"Error while stopping server: {ex.Message}");
        }
    }
}