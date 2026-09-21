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
        /// Đóng gói đối tượng ChatMessage kèm 4-byte Header độ dài (Member 2 Server Core)
        /// </summary>
        public static byte[] Serialize(ChatMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(payloadBytes.Length);

            byte[] fullPacket = new byte[4 + payloadBytes.Length];
            Array.Copy(lengthBytes, 0, fullPacket, 0, 4);
            Array.Copy(payloadBytes, 0, fullPacket, 4, payloadBytes.Length);

            return fullPacket;
        }

        /// <summary>
        /// Giải mã mảng byte Payload (đã tách 4-byte Header độ dài) thành đối tượng ChatMessage.
        /// Tự động thích ứng cả định dạng ChatMessage trực tiếp và BasePacket.
        /// </summary>
        public static ChatMessage? Deserialize(byte[] payloadBytes)
        {
            if (payloadBytes == null || payloadBytes.Length == 0) return null;
            string json = Encoding.UTF8.GetString(payloadBytes);
            return Deserialize(json);
        }

        /// <summary>
        /// Giải mã chuỗi JSON thành ChatMessage, hỗ trợ chuyển đổi từ BasePacket nếu cần
        /// </summary>
        public static ChatMessage? Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Nếu gói tin là dạng BasePacket (SenderId / PayloadJson)
                if (root.TryGetProperty("SenderId", out var senderIdProp) || root.TryGetProperty("PayloadJson", out _))
                {
                    var basePacket = JsonSerializer.Deserialize<BasePacket>(json);
                    if (basePacket != null)
                    {
                        // Thử xem PayloadJson có chứa ChatMessage lồng bên trong không
                        if (!string.IsNullOrEmpty(basePacket.PayloadJson))
                        {
                            try
                            {
                                var innerMsg = JsonSerializer.Deserialize<ChatMessage>(basePacket.PayloadJson);
                                if (innerMsg != null && (!string.IsNullOrEmpty(innerMsg.SenderUsername) || !string.IsNullOrEmpty(innerMsg.Content)))
                                {
                                    if (string.IsNullOrEmpty(innerMsg.SenderUsername))
                                        innerMsg.SenderUsername = basePacket.SenderId;
                                    innerMsg.Type = basePacket.Type;
                                    return innerMsg;
                                }
                            }
                            catch { }

                            // Thử xem PayloadJson có phải là đối tượng User không (gói CONNECT)
                            try
                            {
                                var user = JsonSerializer.Deserialize<User>(basePacket.PayloadJson);
                                if (user != null && !string.IsNullOrEmpty(user.Username))
                                {
                                    return new ChatMessage
                                    {
                                        Type = basePacket.Type,
                                        SenderUsername = user.Username,
                                        Content = basePacket.PayloadJson
                                    };
                                }
                            }
                            catch { }
                        }

                        return new ChatMessage
                        {
                            Type = basePacket.Type,
                            SenderUsername = basePacket.SenderId,
                            Content = basePacket.PayloadJson
                        };
                    }
                }

                // Gói tin dạng ChatMessage trực tiếp
                return JsonSerializer.Deserialize<ChatMessage>(json);
            }
            catch
            {
                return null;
            }
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
            try
            {
                var packet = JsonSerializer.Deserialize<BasePacket>(json);
                if (packet != null && string.IsNullOrEmpty(packet.SenderId))
                {
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("SenderUsername", out var senderUserProp))
                    {
                        packet.SenderId = senderUserProp.GetString() ?? string.Empty;
                    }
                    if (string.IsNullOrEmpty(packet.PayloadJson) && doc.RootElement.TryGetProperty("Content", out var contentProp))
                    {
                        packet.PayloadJson = contentProp.GetString() ?? string.Empty;
                    }
                }
                return packet;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<byte[]?> ReadExactBytesAsync(NetworkStream stream, int count, CancellationToken ct = default)
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