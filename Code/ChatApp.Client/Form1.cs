using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatApp.Client.Network;
using ChatApp.Client.UI;
using ChatApp.Shared.Models;

namespace ChatApp.Client
{
    public partial class Form1 : Form
    {
        private readonly TcpClientService _clientService;
        private string _myUsername = "";
        private string? _myAvatarBase64 = null;
        private string _selectedReceiver = "ALL";
        private readonly List<User> _onlineUsers = new();

        public Form1()
        {
            InitializeComponent();
            _clientService = new TcpClientService();

            SetupEventHandlers();
            InitializeDefaultValues();
        }

        private void InitializeDefaultValues()
        {
            txtUsername.Text = "User_" + new Random().Next(100, 999);
            lstContacts.Items.Add("📢 PHÒNG CHUNG (ALL)");
            lstContacts.SelectedIndex = 0;
            chatBox.SetChatTitle("💬 Phòng chat chung (ALL)");
        }

        private void SetupEventHandlers()
        {
            btnConnect.Click += async (s, e) => await ConnectToServerAsync();
            btnDisconnect.Click += (s, e) => DisconnectFromServer();
            btnAvatar.Click += async (s, e) => await UploadAvatarAsync();
            lstContacts.SelectedIndexChanged += LstContacts_SelectedIndexChanged;

            chatBox.MessageSent += async (s, msg) => await SendChatMessageAsync(msg);
            chatBox.ForwardRequested += async (s, msg) => await HandleForwardMessageAsync(msg);

            _clientService.OnConnectionStatusChanged += ClientService_OnConnectionStatusChanged;
            _clientService.OnMessageReceived += ClientService_OnMessageReceived;

            this.FormClosing += (s, e) => DisconnectFromServer();
        }

        private async Task ConnectToServerAsync()
        {
            string username = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng trước khi kết nối!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            string ip = txtServerIP.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                port = 5000;
            }

            _myUsername = username;
            btnConnect.Enabled = false;
            lblStatus.Text = "🟡 Đang kết nối...";
            lblStatus.ForeColor = Color.Orange;

            await _clientService.ConnectAsync(ip, port);

            if (_clientService.IsConnected)
            {
                // Gửi gói tin CONNECT để đăng ký tên lên Server
                var connectPacket = new ChatMessage
                {
                    Type = MessageType.CONNECT,
                    SenderUsername = _myUsername,
                    Content = _myUsername,
                    AvatarBase64 = _myAvatarBase64,
                    Timestamp = DateTime.Now
                };

                await _clientService.SendMessageAsync(connectPacket);
            }
        }

        private void DisconnectFromServer()
        {
            if (_clientService.IsConnected)
            {
                _clientService.Disconnect();
            }

            UpdateUIOnDisconnected();
        }

        private void ClientService_OnConnectionStatusChanged(bool isConnected, string message)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke(() =>
            {
                if (isConnected)
                {
                    lblStatus.Text = "🟢 Đã kết nối";
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    txtServerIP.Enabled = false;
                    txtPort.Enabled = false;
                    txtUsername.Enabled = false;
                    chatBox.AddTextMessage("HỆ THỐNG", $"Kết nối máy chủ thành công với tên: {_myUsername}");
                }
                else
                {
                    UpdateUIOnDisconnected();
                    chatBox.AddTextMessage("HỆ THỐNG", message);
                }
            });
        }

        private void UpdateUIOnDisconnected()
        {
            lblStatus.Text = "🔴 Chưa kết nối";
            lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            txtServerIP.Enabled = true;
            txtPort.Enabled = true;
            txtUsername.Enabled = true;

            _onlineUsers.Clear();
            lstContacts.Items.Clear();
            lstContacts.Items.Add("📢 PHÒNG CHUNG (ALL)");
            lstContacts.SelectedIndex = 0;
            lblContactHeader.Text = "ONLINE USERS [0]";
        }

        private void ClientService_OnMessageReceived(ChatMessage message)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke(() =>
            {
                switch (message.Type)
                {
                    case MessageType.CHAT_TEXT:
                    case MessageType.CHAT_REPLY:
                    case MessageType.CHAT_FORWARD:
                        chatBox.AddMessage(message, isSentByMe: false);
                        break;

                    case MessageType.GET_ONLINE_USERS:
                        ProcessOnlineUsers(message.Content);
                        break;

                    case MessageType.CONNECT:
                        if (message.SenderUsername != _myUsername)
                        {
                            AddOnlineUser(message.SenderUsername, message.AvatarBase64);
                            chatBox.AddTextMessage("HỆ THỐNG", $"🟢 {message.SenderUsername} vừa tham gia phòng chat.");
                        }
                        break;

                    case MessageType.DISCONNECT:
                        RemoveOnlineUser(message.SenderUsername);
                        chatBox.AddTextMessage("HỆ THỐNG", $"⚪ {message.SenderUsername} đã rời phòng chat.");
                        break;

                    case MessageType.UPDATE_AVATAR:
                        UpdateUserAvatar(message.SenderUsername, message.AvatarBase64);
                        break;

                    case MessageType.ERROR:
                        MessageBox.Show(message.Content, "Thông báo từ Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            });
        }

        private void ProcessOnlineUsers(string json)
        {
            try
            {
                var users = JsonSerializer.Deserialize<List<User>>(json);
                if (users != null)
                {
                    _onlineUsers.Clear();
                    _onlineUsers.AddRange(users);
                    RefreshContactListBox();
                }
            }
            catch { }
        }

        private void AddOnlineUser(string username, string? avatarBase64)
        {
            if (!_onlineUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                _onlineUsers.Add(new User { Username = username, DisplayName = username, AvatarBase64 = avatarBase64, IsOnline = true });
                RefreshContactListBox();
            }
        }

        private void RemoveOnlineUser(string username)
        {
            _onlineUsers.RemoveAll(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            RefreshContactListBox();
        }

        private void UpdateUserAvatar(string username, string? avatarBase64)
        {
            var user = _onlineUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.AvatarBase64 = avatarBase64;
            }
        }

        private void RefreshContactListBox()
        {
            string currentSelected = _selectedReceiver;

            lstContacts.Items.Clear();
            lstContacts.Items.Add("📢 PHÒNG CHUNG (ALL)");

            foreach (var user in _onlineUsers.Where(u => !u.Username.Equals(_myUsername, StringComparison.OrdinalIgnoreCase)))
            {
                lstContacts.Items.Add($"🟢 {user.Username}");
            }

            lblContactHeader.Text = $"ONLINE USERS [{_onlineUsers.Count}]";

            // Khôi phục lựa chọn
            int foundIndex = 0;
            if (currentSelected != "ALL")
            {
                for (int i = 1; i < lstContacts.Items.Count; i++)
                {
                    if (lstContacts.Items[i].ToString()?.EndsWith(currentSelected) == true)
                    {
                        foundIndex = i;
                        break;
                    }
                }
            }
            lstContacts.SelectedIndex = foundIndex;
        }

        private void LstContacts_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstContacts.SelectedIndex <= 0)
            {
                _selectedReceiver = "ALL";
                chatBox.SetChatTitle("💬 Phòng chat chung (ALL)");
            }
            else
            {
                string raw = lstContacts.SelectedItem?.ToString() ?? "";
                string target = raw.Replace("🟢", "").Trim();
                _selectedReceiver = target;
                chatBox.SetChatTitle($"🔒 Đang chat riêng với: {target}");
            }
        }

        private async Task SendChatMessageAsync(ChatMessage message)
        {
            if (!_clientService.IsConnected)
            {
                MessageBox.Show("Bạn chưa kết nối tới Server!", "Chưa kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            message.SenderUsername = _myUsername;
            message.ReceiverUsername = _selectedReceiver;
            message.AvatarBase64 = _myAvatarBase64;

            bool sent = await _clientService.SendMessageAsync(message);
            if (sent)
            {
                chatBox.AddMessage(message, isSentByMe: true);
            }
        }

        private async Task HandleForwardMessageAsync(ChatMessage origMessage)
        {
            if (!_clientService.IsConnected) return;

            var userNames = _onlineUsers.Select(u => u.Username).ToList();
            using var formForward = new FrmForwardMessage(userNames, _myUsername);
            if (formForward.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(formForward.SelectedUsername))
            {
                var fwdMessage = new ChatMessage
                {
                    Type = MessageType.CHAT_FORWARD,
                    SenderUsername = _myUsername,
                    ReceiverUsername = formForward.SelectedUsername,
                    ForwardFromUser = string.IsNullOrEmpty(origMessage.ForwardFromUser) ? origMessage.SenderUsername : origMessage.ForwardFromUser,
                    Content = origMessage.Content,
                    AvatarBase64 = _myAvatarBase64,
                    Timestamp = DateTime.Now
                };

                await _clientService.SendMessageAsync(fwdMessage);
                chatBox.AddTextMessage("CHUYỂN TIẾP", $"➤ Bạn đã chuyển tiếp tin nhắn tới {formForward.SelectedUsername}: \"{origMessage.Content}\"", isSentByMe: true);
            }
        }

        private async Task UploadAvatarAsync()
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Hình ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn ảnh đại diện Avatar"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    byte[] bytes = await File.ReadAllBytesAsync(dialog.FileName);
                    if (bytes.Length > 50 * 1024)
                    {
                        // Resize nếu ảnh quá lớn (>50KB) để vừa chuẩn Server MaxAvatarSizeKB
                        using var ms = new MemoryStream(bytes);
                        using var img = Image.FromStream(ms);
                        using var bmp = new Bitmap(img, new Size(64, 64));
                        using var outMs = new MemoryStream();
                        bmp.Save(outMs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        bytes = outMs.ToArray();
                    }

                    _myAvatarBase64 = Convert.ToBase64String(bytes);

                    if (_clientService.IsConnected)
                    {
                        var avatarMsg = new ChatMessage
                        {
                            Type = MessageType.UPDATE_AVATAR,
                            SenderUsername = _myUsername,
                            AvatarBase64 = _myAvatarBase64,
                            Timestamp = DateTime.Now
                        };
                        await _clientService.SendMessageAsync(avatarMsg);
                    }

                    MessageBox.Show("Cập nhật Avatar thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể đọc ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
