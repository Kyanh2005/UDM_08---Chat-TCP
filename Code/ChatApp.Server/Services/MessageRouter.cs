using System.Collections.Concurrent;
using ChatApp.Server.Network;
using ChatApp.Shared.Models;
using System.Text.Json;

namespace ChatApp.Server.Services
{
    // ========================================================================================
    // FILE: MessageRouter.cs - TỔNG ĐÀI ĐỊNH TUYẾN VÀ PHÂN PHỐI TIN NHẮN (MEMBER 2)
    // 
    // - VAI TRÒ: 
    //   1. Nhận các gói tin hợp lệ từ TcpServerListener.
    //   2. Phân loại theo MessageType để xử lý tương ứng:
    //      - CONNECT: Đăng ký User, gửi danh sách Online, Broadcast cho người khác biết.
    //      - DISCONNECT: Xóa Session, thông báo cho toàn bộ phòng chat.
    //      - CHAT_TEXT / CHAT_REPLY / CHAT_FORWARD: Broadcast (gửi tất cả) hoặc Unicast (gửi đích danh 1 người).
    //      - UPDATE_AVATAR: Cập nhật avatar và phát sóng avatar mới cho mọi người.
    //      - GET_ONLINE_USERS: Đóng gói JSON danh sách đang online gửi về Client.
    // - KẾT NỐI VỚI:
    //   + Member 1 (Shared Models): Sử dụng các model User, ChatMessage, MessageType.
    //   + Member 3 (Client Network): Nhận yêu cầu và gửi dữ liệu phản hồi về Client qua ClientSession.
    //   + Member 4 (Client UI - Contact List): Cung cấp dữ liệu danh sách Online Users để render danh bạ.
    //   + Member 5 (Client UI - Chat View): Chuyển tiếp tin nhắn thường, tin Reply (Quote), tin Forward.
    // ========================================================================================
    public class MessageRouter
    {
        private readonly ConcurrentDictionary<string, ClientSession> _sessions;

        public MessageRouter(ConcurrentDictionary<string, ClientSession> sessions)
        {
            _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
        }
        
        // ====================================================================================
        // HÀM: Phân loại gói tin và điều hướng xử lý theo MessageType
        // ====================================================================================
        public async Task RouterMessageAsync(ClientSession senderSession, ChatMessage message)
        {
            switch (message.Type)
            {
                case MessageType.CONNECT:
                    await HandleConnectAsync(senderSession, message);
                    break;
                case MessageType.DISCONNECT:
                    await HandleDisconnectAsync(senderSession);
                    break;
                case MessageType.CHAT_TEXT:
                case MessageType.CHAT_REPLY:
                case MessageType.CHAT_FORWARD:
                    await HandleChatMessageAsync(senderSession, message);
                    break;
                
                case MessageType.UPDATE_AVATAR:
                    await HandleUpdateAvatarAsync(senderSession, message);
                    break;
                case MessageType.GET_ONLINE_USERS:
                    await HandleGetOnlineUsersAsync(senderSession);
                    break;

                default:
                    ServerLogger.LogWarn($"Unknown MessageType '{message.Type}' from {senderSession.RemoteEndPoint}");
                    break;
            }
        }

        // ====================================================================================
        // HÀM: Xử lý Đăng ký người dùng khi vừa kết nối vào Server
        // ====================================================================================
        private async Task HandleConnectAsync(ClientSession session, ChatMessage message)
        {
            string username = message.SenderUsername.Trim();

            if (_sessions.ContainsKey(username))
            {
                ServerLogger.LogWarn($"Registration rejected: Username '{username}' already connected.", session.RemoteEndPoint);
                var errorPacket = new ChatMessage
                {
                    Type = MessageType.ERROR,
                    SenderUsername = "SERVER",
                    ReceiverUsername = username,
                    Content = "ERROR_400_INVALID_DATA: Username is already in use by another active session."
                };
                await session.SendMessageAsync(errorPacket);
                return;
            }

            session.Username = username;
            session.DisplayName = !string.IsNullOrWhiteSpace(message.Content) ? message.Content : username;
            session.AvatarBase64 = message.AvatarBase64;

            _sessions[username] = session;
            ServerLogger.LogInfo($"User '{username}' connected successfully ({_sessions.Count} online)", session.RemoteEndPoint);

            await SendOnlineUsersToClientAsync(session);

            var broadcastJoin = new ChatMessage
            {
                Type = MessageType.CONNECT,
                SenderUsername = username,
                Content = session.DisplayName,
                AvatarBase64 = session.AvatarBase64,
                Timestamp = DateTime.Now
            };
            await BroadcastExceptAsync(broadcastJoin, username);
        }
        
        // ====================================================================================
        // HÀM: Xử lý khi Client ngắt kết nối (Chủ động thoát hoặc mất mạng đột ngột)
        // ====================================================================================
        public async Task HandleDisconnectAsync(ClientSession session)
        {
            if (string.IsNullOrEmpty(session.Username)) return;

            if (_sessions.TryRemove(session.Username, out _))
            {
                ServerLogger.LogWarn($"User '{session.Username}' disconnected. ({_sessions.Count} online)", session.RemoteEndPoint);

                var offlineNotice = new ChatMessage
                {
                    Type = MessageType.DISCONNECT,
                    SenderUsername = session.Username,
                    Content = $"User {session.Username} has left the chat.",
                    Timestamp = DateTime.Now
                };
                await BroadcastAsync(offlineNotice);
            }
        }

        // ====================================================================================
        // HÀM: Xử lý chuyển tiếp tin nhắn Text, Reply, Forward (Hỗ trợ Broadcast & Unicast)
        // ====================================================================================
        private async Task HandleChatMessageAsync(ClientSession senderSession, ChatMessage message)
        {
            // TRƯỜNG HỢP 1: Gửi tin nhắn chung cho cả phòng (Broadcast)
            if (string.IsNullOrWhiteSpace(message.ReceiverUsername) ||
                message.ReceiverUsername.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
                message.ReceiverUsername == "*")
            {
                ServerLogger.LogInfo($"[BROADCAST] From: {message.SenderUsername} | Content: {message.Content}");
                // Gửi cho tất cả mọi người ngoại trừ chính người gửi
                await BroadcastExceptAsync(message, senderSession.Username);
            }
             // TRƯỜNG HỢP 2: Chat riêng 1-1 đích danh (Unicast)
            else
            {
                string targetUser = message.ReceiverUsername.Trim();
                if (_sessions.TryGetValue(targetUser, out var receiverSession))
                {
                    ServerLogger.LogInfo($"[UNICAST] From: {message.SenderUsername} -> To: {targetUser} | Content: {message.Content}");
                    await receiverSession.SendMessageAsync(message);
                }
                else
                {
                    // Người nhận hiện không online -> Báo lỗi về cho người gửi
                    ServerLogger.LogWarn($"[UNICAST FAILED] Target user '{targetUser}' not online.", senderSession.RemoteEndPoint);
                    var notFoundPacket = new ChatMessage
                    {
                        Type = MessageType.ERROR,
                        SenderUsername = "SERVER",
                        ReceiverUsername = senderSession.Username,
                        Content = $"ERROR_404_USER_NOT_FOUND: User '{targetUser}' is currently offline."
                    };
                    await senderSession.SendMessageAsync(notFoundPacket);
                }
            }
        
        }
        // ====================================================================================
        // HÀM: Xử lý cập nhật Avatar của người dùng và thông báo cho mọi người
        // ====================================================================================
        private async Task HandleUpdateAvatarAsync(ClientSession senderSession, ChatMessage message)
        {
            senderSession.AvatarBase64 = message.AvatarBase64;
            ServerLogger.LogInfo($"User '{senderSession.Username}' updated avatar.", senderSession.RemoteEndPoint);

            var avatarPacket = new ChatMessage
            {
                Type = MessageType.UPDATE_AVATAR,
                SenderUsername = senderSession.Username,
                AvatarBase64 = message.AvatarBase64,
                Timestamp = DateTime.Now
            };
            await BroadcastAsync(avatarPacket);
        }
        // ====================================================================================
        // HÀM: Xử lý yêu cầu lấy danh sách người dùng đang Online (Kết nối với Member 4 - ucContactList)
        // ====================================================================================
        private async Task HandleGetOnlineUsersAsync(ClientSession senderSession)
        {
            await SendOnlineUsersToClientAsync(senderSession);
        }
        
        // ====================================================================================
        // HÀM: Đóng gói toàn bộ User đang online thành chuỗi JSON và gửi riêng về cho 1 Client
        // ====================================================================================
        private async Task SendOnlineUsersToClientAsync(ClientSession targetSession)
        {
            var userList = _sessions.Values.Select(s => new User
            {
                Username = s.Username,
                DisplayName = s.DisplayName,
                AvatarBase64 = s.AvatarBase64,
                IsOnline = true
            }).ToList();

            string jsonUsers = JsonSerializer.Serialize(userList);

            var onlineUsersPacket = new ChatMessage
            {
                Type = MessageType.GET_ONLINE_USERS,
                SenderUsername = "SERVER",
                ReceiverUsername = targetSession.Username,
                Content = jsonUsers,
                Timestamp = DateTime.Now
            };
            await targetSession.SendMessageAsync(onlineUsersPacket);
        }
         // ====================================================================================
        // HÀM BỔ TRỢ: Bắn gói tin tới TẤT CẢ mọi người đang kết nối
        // ====================================================================================
        public async Task BroadcastAsync(ChatMessage message)
        {
            var tasks = _sessions.Values.Select(s => s.SendMessageAsync(message));
            await Task.WhenAll(tasks);
        }
        // ====================================================================================
        // HÀM BỔ TRỢ: Bắn gói tin tới tất cả mọi người TRỪ 1 người ra (thường là trừ người gửi)
        // ====================================================================================
        public async Task BroadcastExceptAsync(ChatMessage message, string exceptUsername)
        {
            var targetSessions = _sessions.Values.Where(s => !s.Username.Equals(exceptUsername, StringComparison.OrdinalIgnoreCase));
            var tasks = targetSessions.Select(s => s.SendMessageAsync(message));
            await Task.WhenAll(tasks);
        }

    }
}