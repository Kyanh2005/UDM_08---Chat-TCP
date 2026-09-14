namespace ChatApp.Shared.Models
{
    /// <summary>
    /// Model đại diện cho thông tin tin nhắn được Forward (Chuyển tiếp) - Member 1
    /// </summary>
    public class ForwardPayload
    {
        public string ForwardFromUser { get; set; } = string.Empty;
        public string OriginalContent { get; set; } = string.Empty;
        public DateTime OriginalTimestamp { get; set; } = DateTime.Now;
    }
}
