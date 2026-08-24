using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Client.Network
{
    public class TcpClientService
    {
    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cts;

    // Sự kiện gửi dữ liệu hoặc trạng thái về UI Thread
    public event Action<string> OnDataReceived;
    public event Action<bool, string> OnConnectionStatusChanged;

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

            // BẮT BỘC: Cấu hình ReadTimeout và WriteTimeout
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
        byte[] buffer = new byte[4096];

        try
        {
            while (!token.IsCancellationRequested && IsConnected)
            {
                // Sử dụng ReadAsync kết hợp CancellationToken để dừng nhận dữ liệu tức thì khi disconnect
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                if (bytesRead == 0)
                {
                    // Server ngắt kết nối chủ động (FIN packet)
                    throw new SocketException((int)SocketError.ConnectionReset);
                }

                // Giải mã packet nhận từ Mem 1 (Ví dụ: Decode UTF8 hoặc Binary Protocol của Mem 1)
                string rawData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                // Đẩy dữ liệu ra Event (UI sẽ subscribe event này)
                OnDataReceived?.Invoke(rawData);
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

    public async Task SendDataAsync(string message)
    {
        if (!IsConnected || _stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            OnConnectionStatusChanged?.Invoke(false, $"Lỗi khi gửi dữ liệu: {ex.Message}");
            Cleanup();
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