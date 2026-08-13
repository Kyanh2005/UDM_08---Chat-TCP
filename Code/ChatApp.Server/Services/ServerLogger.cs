namespace ChatApp.Server.Services
{
    public static class ServerLogger
    {
        public static void Log(string message)
        {
            string formattedLog = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(formattedLog);
        }
    }
}