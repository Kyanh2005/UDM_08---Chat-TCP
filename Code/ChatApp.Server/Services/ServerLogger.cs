using System.Text.RegularExpressions;

namespace ChatApp.Server.Services
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error,
        Success
    }
    public static class ServerLogger
    {
        private static readonly object _fileLock = new();
        private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static event Action<string, LogLevel>? OnLog;

        static ServerLogger()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServerLogger Init Error]: {ex.Message}");
            }
        }
        public static void Log(string message, string endpoint = "SERVER", LogLevel level = LogLevel.Info)
        {
            string sanitized = SanitizeSensitiveData(message);
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedLog = $"[{timeStamp}] [{endpoint}] [{level.ToString().ToUpper()}] {sanitized}";

            PrintConsole(formattedLog, level);
            WriteToFile(formattedLog);

            try
            {
                OnLog?.Invoke(formattedLog, level);
            }
            catch { }
        }
        public static void LogInfo(string message, string endpoint = "SERVER") => Log(message, endpoint, LogLevel.Info);
        public static void LogWarn(string message, string endpoint = "SERVER") => Log(message, endpoint, LogLevel.Warn);
        public static void LogError(string message, string endpoint = "SERVER") => Log(message, endpoint, LogLevel.Error);
        public static void LogSuccess(string message, string endpoint = "SERVER") => Log(message, endpoint, LogLevel.Success);

        private static string SanitizeSensitiveData(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            if (input.Length > 200)
            {
                return input.Substring(0, 200) + "... [TRUNCATED - DỮ LIỆU DÀI ĐÃ ĐƯỢC RÚT GỌN]";
            }
            return input;
        }
        private static void PrintConsole(string text, LogLevel level)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            switch (level)
            {
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Info:
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }
        private static void WriteToFile(string formattedLog)
        {
            lock (_fileLock)
            {
                try
                {
                    string filePath = Path.Combine(_logDirectory, $"server_{DateTime.Now:yyyy-MM-dd}.log");
                    File.AppendAllText(filePath, formattedLog + Environment.NewLine);
                }
                catch
                {
                    
                }
            }
        }
    }
}