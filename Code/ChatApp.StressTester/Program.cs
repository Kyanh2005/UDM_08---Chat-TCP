using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared.Models;
using ChatApp.Shared.Packets;

namespace ChatApp.StressTester
{
    internal class Program
    {
        private static string serverIp = "127.0.0.1";
        private static int serverPort = 5000;

        private static int totalConnected = 0;
        private static int totalSentMessages = 0;
        private static int totalErrorMessages = 0;

        static async Task Main(string[] args)
        {
            Console.Title = "TCP Stress Test Tool - Bot Client";
            Console.WriteLine("=== STRESS TEST BOT CLIENT ===");
            Console.Write("IP Server (Default 127.0.0.1): ");
            string? inputIp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputIp)) serverIp = inputIp;

            Console.Write("Port Server (Default 5000): ");
            string? inputPort = Console.ReadLine();
            if (int.TryParse(inputPort, out int p)) serverPort = p;

            Console.WriteLine("\nChọn chế độ Stress Test:");
            Console.WriteLine("1. Level 1: Tải vừa (20 Bot, gửi tin mỗi 1 giây)");
            Console.WriteLine("2. Level 2: Tải cao (100 Bot, Spam liên tục)");
            Console.WriteLine("3. Level 3: Gửi tin rác/Lỗi format (Test Data Validator)");
            Console.Write("Lựa chọn (1, 2 hoặc 3): ");

            string choice = Console.ReadLine() ?? "1";
            var cts = new CancellationTokenSource();

            _ = Task.Run(() => DisplayStatsLoop(cts.Token));

            if (choice == "1") await StartStressTest(20, 1000, false, cts.Token);
            else if (choice == "2") await StartStressTest(100, 10, false, cts.Token);
            else await StartStressTest(10, 500, sendCorruptedData: true, cts.Token);

            Console.ReadLine();
            cts.Cancel();
        }

        private static async Task StartStressTest(int botCount, int delayMs, bool sendCorruptedData, CancellationToken ct)
        {
            for (int i = 1; i <= botCount; i++)
            {
                int botId = i;
                _ = Task.Run(() => RunBotInstance(botId, delayMs, sendCorruptedData, ct), ct);
                await Task.Delay(10, ct);
            }
        }

        private static async Task RunBotInstance(int botId, int delayMs, bool sendCorruptedData, CancellationToken ct)
        {
            string botName = $"Bot_{botId}";
            try
            {
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);
                using NetworkStream stream = client.GetStream();

                Interlocked.Increment(ref totalConnected);

                _ = Task.Run(async () =>
                {
                    while (!ct.IsCancellationRequested && client.Connected)
                    {
                        var packet = await PacketSerializer.ReceivePacketAsync(stream, ct);
                        if (packet == null) break;
                        if (packet.Type == MessageType.ERROR) Interlocked.Increment(ref totalErrorMessages);
                    }
                }, ct);

                await PacketSerializer.SendPacketAsync(
                    stream,
                    MessageType.CONNECT,
                    botName,
                    new User { Username = botName, DisplayName = botName, IsOnline = true },
                    ct
                );

                while (!ct.IsCancellationRequested)
                {
                    if (sendCorruptedData)
                    {
                        byte[] junk = Encoding.UTF8.GetBytes("INVALID_RAW_DATA_12345");
                        await stream.WriteAsync(junk, 0, junk.Length, ct);
                    }
                    else
                    {
                        var msg = new ChatMessage
                        {
                            SenderUsername = botName,
                            Content = $"Ping from {botName}"
                        };
                        await PacketSerializer.SendPacketAsync(stream, MessageType.CHAT_TEXT, botName, msg, ct);
                    }

                    Interlocked.Increment(ref totalSentMessages);
                    if (delayMs > 0) await Task.Delay(delayMs, ct);
                }
            }
            catch { }
            finally
            {
                Interlocked.Decrement(ref totalConnected);
            }
        }

        private static async Task DisplayStatsLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Clear();
                Console.WriteLine("=== BẢNG THỐNG KÊ STRESS TEST (BẤM ENTER ĐỂ DỪNG) ===");
                Console.WriteLine($"• Bot dang ket noi : {totalConnected}");
                Console.WriteLine($"• Tong tin da gui  : {totalSentMessages}");
                Console.WriteLine($"• Phan hoi Loi/Error: {totalErrorMessages}");
                Console.WriteLine("=======================================================");
                await Task.Delay(1000, ct);
            }
        }
    }
}