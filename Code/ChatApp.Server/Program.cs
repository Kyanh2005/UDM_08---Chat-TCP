using System.Configuration;
using ChatApp.Server.Network;
using ChatApp.Server.Services;

namespace ChatApp.Server
{
    // ========================================================================================
    // FILE: Program.cs - ĐIỂM BẮT ĐẦU CHẠY CỦA SERVER (MEMBER 2)
    // 
    // - VAI TRÒ: Khởi động Server, nạp file cấu hình App.config, lắng nghe tín hiệu tắt máy.
    // - KẾT NỐI VỚI:
    //   + App.config: Đọc IP, Port, giới hạn ký tự (không hard-code theo yêu cầu Mem 2).
    //   + TcpServerListener: Khởi chạy bộ lắng nghe kết nối Socket TCP.
    //   + ServerLogger: Ghi log khởi động hệ thống.
    // ========================================================================================
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "TCP Chat Server - Engine & Logging Subsystem";

            // In banner giao diện Server
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 =================================================================
   ██████╗██╗  ██╗ █████╗ ████████╗    ███████╗███████╗██████╗ ██╗   ██╗███████╗██████╗ 
  ██╔════╝██║  ██║██╔══██╗╚══██╔══╝    ██╔════╝██╔════╝██╔══██╗██║   ██║██╔════╝██╔══██╗
  ██║     ███████║███████║   ██║       ███████╗█████╗  ██████╔╝██║   ██║█████╗  ██████╔╝
  ██║     ██╔══██║██╔══██║   ██║       ╚════██║██╔══╝  ██╔══██╗╚██╗ ██╔╝██╔══╝  ██╔══██╗
  ╚██████╗██║  ██║██║  ██║   ██║       ███████║███████╗██║  ██║ ╚████╔╝ ███████╗██║  ██║
   ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝       ╚══════╝╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚══════╝╚═╝  ╚═╝
 =================================================================
            ");
            Console.ResetColor();

            // 1. Đọc cấu hình từ file App.config (Nếu không có thì dùng giá trị mặc định)
            string ipAddress = ConfigurationManager.AppSettings["ServerIP"] ?? "127.0.0.1";
            int port = 5000;
            int maxMessageLength = 5000;
            int maxAvatarSizeKB = 50;

            if (int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out int configPort))
            {
                port = configPort;
            }

            if (int.TryParse(ConfigurationManager.AppSettings["MaxMessageLength"], out int configMaxMsg))
            {
                maxMessageLength = configMaxMsg;
            }

            if (int.TryParse(ConfigurationManager.AppSettings["MaxAvatarSizeKB"], out int configMaxAvatar))
            {
                maxAvatarSizeKB = configMaxAvatar;
            }

            // 2. Cho phép người dùng truyền tham số từ dòng lệnh (nếu muốn ghi đè IP/Port)
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                ipAddress = args[0];
            }
            if (args.Length > 1 && int.TryParse(args[1], out int cliPort))
            {
                port = cliPort;
            }

            // 3. Ghi log cấu hình đã nạp
            ServerLogger.LogInfo($"Configuration loaded: IP = {ipAddress}, Port = {port}");
            ServerLogger.LogInfo($"Constraints: Max Message Length = {maxMessageLength} chars, Max Avatar = {maxAvatarSizeKB} KB");
            ServerLogger.LogInfo("Starting TCP Server Core...");

            // 4. Khởi tạo đối tượng TcpServerListener để bắt đầu mở cổng lắng nghe kết nối
            var server = new TcpServerListener(ipAddress, port, maxMessageLength, maxAvatarSizeKB);

            // 5. Bắt sự kiện bấm Ctrl+C để tắt Server an toàn (giải phóng toàn bộ socket trước khi thoát)
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                ServerLogger.LogWarn("Shutdown signal received (Ctrl+C). Closing server...");
                server.Stop();
                Environment.Exit(0);
            };

            // 6. Chạy Server ở chế độ bất đồng bộ
            await server.StartAsync();
        }
    }
}
