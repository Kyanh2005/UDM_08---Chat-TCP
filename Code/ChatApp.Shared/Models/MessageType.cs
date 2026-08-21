namespace ChatApp.Shared.Models
{
    public enum MessageType
    {
        CONNECT = 1,
        DISCONNECT = 2,
        CHAT_TEXT = 3,
        CHAT_REPLY = 4,
        CHAT_FORWARD = 5,
        UPDATE_AVATAR = 6,
        GET_ONLINE_USERS = 7,
        ERROR = 99
    
    }
}