using System;
using System.Drawing;
using System.Windows.Forms;
using ChatApp.Shared.Models;
using ChatApp.Client.UI.Controls;

namespace ChatApp.Client.UI
{
    /// <summary>
    /// UserControl để hiển thị khung chat text cơ bản
    /// Member 5 - Giai đoạn 2: Hoàn thiện giao diện chat
    /// </summary>
    public partial class ChatBoxUserControl : UserControl
    {
        public event EventHandler<string>? MessageSent;

        public ChatBoxUserControl()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Xử lý sự kiện click button Send
            btnSend.Click += BtnSend_Click;

            // Xử lý sự kiện Enter trong textbox
            txtMessage.KeyDown += TxtMessage_KeyDown;
        }

        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            // Gửi tin nhắn khi nhấn Enter (không có Shift)
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true; // Ngăn ký tự Enter xuống dòng
                SendMessage();
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Raise event để gửi message ra ngoài
                MessageSent?.Invoke(this, message);

                // Clear textbox
                txtMessage.Clear();
                txtMessage.Focus();
            }
        }

        /// <summary>
        /// Thêm tin nhắn vào chat box
        /// </summary>
        public void AddMessage(ChatMessage message, bool isSentByMe = false)
        {
            // Tạo bubble cho message
            var bubble = new ucChatMessageBubble();
            
            // TODO: Giai đoạn 3 sẽ format bubble đẹp hơn
            // Tạm thời hiển thị text đơn giản
            Label lblMessage = new Label
            {
                Text = $"{message.SenderUsername}: {message.Content}",
                AutoSize = true,
                MaximumSize = new Size(450, 0),
                Padding = new Padding(10),
                BackColor = isSentByMe ? Color.LightBlue : Color.LightGray,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            bubble.Controls.Clear();
            bubble.Controls.Add(lblMessage);
            bubble.Size = new Size(flowLayoutPanelMessages.Width - 30, lblMessage.Height + 20);
            bubble.BackColor = Color.Transparent;

            // Thêm vào flow panel
            flowLayoutPanelMessages.Controls.Add(bubble);

            // Scroll xuống dưới cùng
            ScrollToBottom();
        }

        /// <summary>
        /// Thêm tin nhắn text đơn giản (dùng cho testing)
        /// </summary>
        public void AddTextMessage(string sender, string content, bool isSentByMe = false)
        {
            var message = new ChatMessage
            {
                SenderUsername = sender,
                Content = content,
                Timestamp = DateTime.Now
            };

            AddMessage(message, isSentByMe);
        }

        /// <summary>
        /// Xóa tất cả tin nhắn
        /// </summary>
        public void ClearMessages()
        {
            flowLayoutPanelMessages.Controls.Clear();
        }

        /// <summary>
        /// Set tiêu đề chat window
        /// </summary>
        public void SetChatTitle(string title)
        {
            lblChatTitle.Text = title;
        }

        private void ScrollToBottom()
        {
            flowLayoutPanelMessages.ScrollControlIntoView(
                flowLayoutPanelMessages.Controls[flowLayoutPanelMessages.Controls.Count - 1]
            );
        }
    }
}
