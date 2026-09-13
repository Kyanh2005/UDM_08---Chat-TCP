namespace ChatApp.Shared.Models
{
    public class ChatMessage
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public MessageType Type { get; set; } = MessageType.CHAT_TEXT;
        public string SenderUsername { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? ReplyToMessageId { get; set; }
        public string? ForwardFromUser { get; set; }
        public string? AvatarBase64 { get; set; }

    }
}