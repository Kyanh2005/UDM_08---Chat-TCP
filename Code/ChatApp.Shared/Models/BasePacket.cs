using System;

namespace ChatApp.Shared.Models
{
    public class BasePacket
    {
        public MessageType Type { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string PayloadJson { get; set; } = string.Empty;
    }
}