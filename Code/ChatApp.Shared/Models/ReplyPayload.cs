namespace ChatApp.Shared.Models
{
    /// <summary>
    /// Model đại diện cho thông tin tin nhắn được Reply (Quote) - Member 1
    /// </summary>
    public class ReplyPayload
    {
        public string ReplyToMessageId { get; set; } = string.Empty;
        public string QuotedSenderUsername { get; set; } = string.Empty;
        public string QuotedContent { get; set; } = string.Empty;
    }
}
