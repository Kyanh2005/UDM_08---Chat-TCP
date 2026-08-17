using ChatApp.Shared.Models;

namespace ChatApp.Server.Services
{
    public static class DataValidator
    {
        public static bool ValidateMessage(ChatMessage message, out string errorMessage)
        {
            if (message == null)
            {
                errorMessage = "Message is null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(message.SenderUsername))
            {
                errorMessage = "Sender username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(message.ReceiverUsername))
            {
                errorMessage = "Receiver username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                errorMessage = "Message content is required.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}