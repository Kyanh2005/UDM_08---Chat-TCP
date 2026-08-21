using ChatApp.Shared.Models;
using System.Text.RegularExpressions;

namespace ChatApp.Server.Services
{
    public static class DataValidator
    {
        public static readonly string ErrorCode = "ERROR_400_INVALID_DATA";
        public static bool ValidateMessage(ChatMessage? message, out string errorMessage, int maxMessageLength = 5000, int maxAvatarSizeKB =50)
        {
            if (message == null || !Enum.IsDefined(typeof(MessageType), message.Type))
            {
                errorMessage = $"{ErrorCode}: Invalid message format or payload.";
                return false;
            }

            string username = message.SenderUsername?.Trim() ?? "";
            if (username.Length < 1 || username.Length > 50 || !Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                errorMessage = $"{ErrorCode}: Invalid SenderUsername (must be 1-50 alphanumeric chars).";
                return false;
            }

            long avatarBytes =(long)((message.AvatarBase64?.Length ?? 0) * 0.75);
            if ((message.Content?.Length ?? 0) > maxMessageLength || avatarBytes > maxAvatarSizeKB * 1024)
            {
                errorMessage = $"{ErrorCode}: Message content or avatar exceeds maxium size limit. ";
                return false;
            }

            if (message.Type == MessageType.CHAT_REPLY && string.IsNullOrWhiteSpace(message.ReplyToMessageId))
            {
                errorMessage = $"{ErrorCode}: ReplyToMessageId is required for CHAT_REPLY";
                return false;
            }
            if (message.Type == MessageType.CHAT_FORWARD && string.IsNullOrWhiteSpace(message.ForwardFromUser))
            {
                errorMessage = $"{ErrorCode}: ForwardFromUser is required for CHAT_FORWARD";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool ValidateReceiver(string receiverUsername, string senderUsername, out string errorMessage)
        {
            receiverUsername = receiverUsername?.Trim() ?? "";

            if (receiverUsername.Equals("ALL", StringComparison.OrdinalIgnoreCase) || receiverUsername == "*")
            {
                errorMessage = string.Empty;
                return true;
            }
            if (receiverUsername.Length < 1 || receiverUsername.Length > 50 || !Regex.IsMatch(receiverUsername, @"^[a-zA-Z0-9_]+$"))
            {
                errorMessage = $"{ErrorCode}: Invalid ReceiverUsername (must be 1-50 alphanumberic chars).";
                return false;
            }
            if (receiverUsername.Equals(senderUsername, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"{ErrorCode}: A user cannot send a message to themselves.";
                return false;
            }

            errorMessage = string.Empty;
            
            return true;
        }
        private static bool ValidateSpecialMessageFields(ChatMessage message, out string errorMessage)
        {
            switch (message.Type)
            {
            case MessageType.CHAT_REPLY:
                    if (string.IsNullOrWhiteSpace(message.ReplyToMessageId))
                    {
                        errorMessage = $"{ErrorCode}: ReplyToMessageId is required for CHAT_REPLY";
                        return false;
                    }
                    break;

                case MessageType.CHAT_FORWARD:
                    if (string.IsNullOrWhiteSpace(message.ForwardFromUser))
                    {
                        errorMessage = $"{ErrorCode}: ForwardFromUser is required for CHAT_FORWARD";
                        return false;
                    }
                    break;

                case MessageType.UPDATE_AVATAR:
                    if (string.IsNullOrWhiteSpace(message.AvatarBase64))
                    {
                        errorMessage = $"{ErrorCode}: AvatarBase64 data is required for UPDATE_AVATAR";
                        return false;
                    }
                    break;
            }
            errorMessage = string.Empty;
            return true;
        }
    }
}
    
