using System.Text;
using System.Text.Json;
using ChatApp.Shared.Models;

namespace ChatApp.Shared.Packets
{
    public static class PacketSerializer
    {
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

        public static ChatMessage? Deserialize(byte[] payloadBytes)
        {
           string json = Encoding.UTF8.GetString(payloadBytes);
           return JsonSerializer.Deserialize<ChatMessage>(json);
        }
    }
}
