using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared.Models;
using ChatApp.Shared;         

namespace ChatApp.Shared.Packets
{
    public static class PacketSerializer
    {
        /// <summary>
        /// Đóng gói dữ liệu kèm 4-byte Header độ dài và gửi qua Stream
        /// </summary>
        public static async Task SendPacketAsync<T>(NetworkStream stream, MessageType type, string senderId, T payload, CancellationToken ct = default)
        {
            var packet = new BasePacket
            {
                Type = type,
                SenderId = senderId,
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            string json = JsonSerializer.Serialize(packet);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(payloadBytes.Length);

            // Gửi 4-byte độ dài trước, sau đó gửi nội dung Payload
            await stream.WriteAsync(lengthBytes, 0, 4, ct);
            await stream.WriteAsync(payloadBytes, 0, payloadBytes.Length, ct);
            await stream.FlushAsync(ct);
        }

        /// <summary>
        /// Đọc chính xác 4-byte Header rồi đọc đủ số byte Payload tương ứng
        /// </summary>
        public static async Task<BasePacket?> ReceivePacketAsync(NetworkStream stream, CancellationToken ct = default)
        {
            byte[]? lengthBuffer = await ReadExactBytesAsync(stream, 4, ct);
            if (lengthBuffer == null) return null; // Client ngắt kết nối

            int payloadLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (payloadLength <= 0) return null;

            byte[]? bodyBuffer = await ReadExactBytesAsync(stream, payloadLength, ct);
            if (bodyBuffer == null) return null; // Gói tin bị đứt đoạn

            string json = Encoding.UTF8.GetString(bodyBuffer);
            return JsonSerializer.Deserialize<BasePacket>(json);
        }

        private static async Task<byte[]?> ReadExactBytesAsync(NetworkStream stream, int count, CancellationToken ct)
        {
            byte[] buffer = new byte[count];
            int totalRead = 0;

            while (totalRead < count)
            {
                int read = await stream.ReadAsync(buffer, totalRead, count - totalRead, ct);
                if (read == 0) return null; // Connection closed
                totalRead += read;
            }

            return buffer;
        }
    }
}