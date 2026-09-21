using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChatApp.Shared.Models;
using ChatApp.Client.UI.Controls;

namespace ChatApp.Client.UI
{
    /// <summary>
    /// UserControl hiển thị toàn bộ khung chat, danh sách bong bóng tin nhắn,
    /// chọn Emoji, thanh Quote Reply và gửi tin nhắn.
    /// </summary>
    public partial class ChatBoxUserControl : UserControl
    {
        public event EventHandler<ChatMessage>? MessageSent;
        public event EventHandler<ChatMessage>? ReplyRequested;
        public event EventHandler<ChatMessage>? ForwardRequested;

        private ChatMessage? _currentReplyingMessage;
        private ucEmojiPicker? _emojiPicker;
        private Panel? _pnlReplyBanner;
        private Label? _lblReplyBannerText;
        private Button? _btnCancelReply;
        private Button? _btnEmoji;
        private readonly Dictionary<string, string> _messageContentCache = new();

        public ChatBoxUserControl()
        {
            InitializeComponent();
            SetupEnhancedUI();
            SetupEventHandlers();
        }

        private void SetupEnhancedUI()
        {
            // 1. Tạo thanh banner thông báo đang Reply
            _pnlReplyBanner = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(235, 243, 250),
                Padding = new Padding(10, 4, 10, 4),
                Visible = false
            };

            _lblReplyBannerText = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "↩ Đang trả lời: "
            };

            _btnCancelReply = new Button
            {
                Dock = DockStyle.Right,
                Width = 30,
                Text = "✕",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.DarkGray,
                Cursor = Cursors.Hand
            };
            _btnCancelReply.FlatAppearance.BorderSize = 0;
            _btnCancelReply.Click += (s, e) => CancelReply();

            _pnlReplyBanner.Controls.Add(_lblReplyBannerText);
            _pnlReplyBanner.Controls.Add(_btnCancelReply);

            // Gắn banner lên trên input area
            this.Controls.Add(_pnlReplyBanner);
            _pnlReplyBanner.BringToFront();

            // 2. Thêm nút chọn Emoji vào input area
            _btnEmoji = new Button
            {
                Text = "😊",
                Dock = DockStyle.Right,
                Width = 45,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12),
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            _btnEmoji.FlatAppearance.BorderSize = 0;
            _btnEmoji.Click += BtnEmoji_Click;

            pnlInputArea.Controls.Add(_btnEmoji);
            _btnEmoji.BringToFront();
            btnSend.BringToFront();

            // 3. Khởi tạo Popup Emoji Picker
            _emojiPicker = new ucEmojiPicker
            {
                Visible = false
            };
            _emojiPicker.EmojiSelected += (s, emoji) =>
            {
                txtMessage.AppendText(emoji);
                txtMessage.Focus();
                _emojiPicker.Visible = false;
            };
            this.Controls.Add(_emojiPicker);
            _emojiPicker.BringToFront();
        }

        private void SetupEventHandlers()
        {
            btnSend.Click += BtnSend_Click;
            txtMessage.KeyDown += TxtMessage_KeyDown;
        }

        private void BtnEmoji_Click(object? sender, EventArgs e)
        {
            if (_emojiPicker == null) return;

            if (_emojiPicker.Visible)
            {
                _emojiPicker.Visible = false;
            }
            else
            {
                // Định vị vị trí picker nằm ngay trên nút emoji
                int x = pnlInputArea.Right - _emojiPicker.Width - 10;
                int y = pnlInputArea.Top - _emojiPicker.Height - 5;
                _emojiPicker.Location = new Point(Math.Max(10, x), Math.Max(10, y));
                _emojiPicker.Visible = true;
                _emojiPicker.BringToFront();
            }
        }

        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            string content = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(content)) return;

            var message = new ChatMessage
            {
                Type = _currentReplyingMessage != null ? MessageType.CHAT_REPLY : MessageType.CHAT_TEXT,
                Content = content,
                ReplyToMessageId = _currentReplyingMessage?.MessageId,
                Timestamp = DateTime.Now
            };

            MessageSent?.Invoke(this, message);

            CancelReply();
            txtMessage.Clear();
            txtMessage.Focus();
        }

        public void StartReply(ChatMessage message)
        {
            _currentReplyingMessage = message;
            if (_pnlReplyBanner != null && _lblReplyBannerText != null)
            {
                string preview = message.Content.Length > 40 ? message.Content.Substring(0, 37) + "..." : message.Content;
                _lblReplyBannerText.Text = $"↩ Đang trả lời {message.SenderUsername}: \"{preview}\"";
                _pnlReplyBanner.Visible = true;
            }
            txtMessage.Focus();
            ReplyRequested?.Invoke(this, message);
        }

        public void CancelReply()
        {
            _currentReplyingMessage = null;
            if (_pnlReplyBanner != null)
            {
                _pnlReplyBanner.Visible = false;
            }
        }

        /// <summary>
        /// Thêm tin nhắn vào chat box với bong bóng chuẩn ucChatMessageBubble
        /// </summary>
        public void AddMessage(ChatMessage message, bool isSentByMe = false)
        {
            if (!string.IsNullOrEmpty(message.MessageId))
            {
                _messageContentCache[message.MessageId] = message.Content;
            }

            string? quotedContent = null;
            if (!string.IsNullOrEmpty(message.ReplyToMessageId) && _messageContentCache.TryGetValue(message.ReplyToMessageId, out var orig))
            {
                quotedContent = orig;
            }

            var bubble = new ucChatMessageBubble();
            int bubbleWidth = Math.Max(350, flowLayoutPanelMessages.ClientSize.Width - 30);
            bubble.Width = bubbleWidth;
            bubble.SetMessage(message, isSentByMe, quotedContent);

            bubble.ReplyClicked += (s, msg) => StartReply(msg);
            bubble.ForwardClicked += (s, msg) => ForwardRequested?.Invoke(this, msg);

            flowLayoutPanelMessages.Controls.Add(bubble);
            ScrollToBottom();
        }

        /// <summary>
        /// Thêm tin nhắn text đơn giản (thông báo hệ thống hoặc test)
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

        public void ClearMessages()
        {
            flowLayoutPanelMessages.Controls.Clear();
            _messageContentCache.Clear();
            CancelReply();
        }

        public void SetChatTitle(string title)
        {
            lblChatTitle.Text = title;
        }

        private void ScrollToBottom()
        {
            if (flowLayoutPanelMessages.Controls.Count > 0)
            {
                flowLayoutPanelMessages.ScrollControlIntoView(
                    flowLayoutPanelMessages.Controls[flowLayoutPanelMessages.Controls.Count - 1]
                );
            }
        }
    }
}
