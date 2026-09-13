namespace ChatApp.Shared.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarBase64 { get; set; }
        public bool IsOnline { get; set; } = true;
    }
}