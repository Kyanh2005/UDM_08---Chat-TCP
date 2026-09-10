using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ChatApp.Shared.Models;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// UserControl hiển thị bong bóng tin nhắn - Member 5
    /// Hỗ trợ: Avatar, Tên, Thời gian, Nội dung, Reply Quote, Reply/Forward buttons
    /// </summary>
    public partial class ucChatMessageBubble : UserControl
    {
        // Events cho Reply và Forward
        public event EventHandler<ChatMessage>? ReplyClicked;
        public event EventHandler<ChatMessage>? ForwardClicked;

        private ChatMessage? _message;
        private bool _isOwnMessage;

        // UI Components
        private PictureBox picAvatar;
        private Label lblUsername;
        private Label lblTimestamp;
        private Label lblContent;
        private Panel pnlQuote;
        private Label lblQuoteContent;
        private Button btnReply;
        private Button btnForward;
        private Panel pnlBubble;

        public ucChatMessageBubble()
        {
            InitializeComponent();
            InitializeUI();
        }

        /// <summary>
        /// Thiết lập dữ liệu tin nhắn và render UI
        /// </summary>
        public void SetMessage(ChatMessage message, bool isOwnMessage, string? quotedContent = null)
        {
            _message = message;
            _isOwnMessage = isOwnMessage;

            // Cập nhật nội dung
            lblUsername.Text = message.SenderUsername;
            lblTimestamp.Text = message.Timestamp.ToString("HH:mm");
            lblContent.Text = message.Content;

            // Hiển thị Quote nếu có Reply
            if (!string.IsNullOrEmpty(message.ReplyToMessageId) && !string.IsNullOrEmpty(quotedContent))
            {
                pnlQuote.Visible = true;
                lblQuoteContent.Text = $"↩ Trả lời: {quotedContent}";
            }
            else
            {
                pnlQuote.Visible = false;
            }

            // Load Avatar (nếu có)
            LoadAvatar(message.AvatarBase64);

            // Cập nhật layout theo hướng tin nhắn
            UpdateLayout();
        }

        private void InitializeUI()
        {
            this.Size = new Size(450, 100);
            this.Padding = new Padding(10);

            // Bubble Container
            pnlBubble = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(350, 0),
                MinimumSize = new Size(200, 60),
                Padding = new Padding(10)
            };

            // Avatar (tròn)
            picAvatar = new PictureBox
            {
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray
            };

            // Username
            lblUsername = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51)
            };

            // Timestamp
            lblTimestamp = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Regular),
                ForeColor = Color.Gray
            };

            // Quote Panel (Reply)
            pnlQuote = new Panel
            {
                AutoSize = true,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(8, 5, 8, 5),
                Visible = false
            };

            lblQuoteContent = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                MaximumSize = new Size(300, 0),
                Dock = DockStyle.Fill
            };

            pnlQuote.Controls.Add(lblQuoteContent);

            // Content
            lblContent = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                MaximumSize = new Size(320, 0),
                ForeColor = Color.Black
            };

            // Reply Button
            btnReply = new Button
            {
                Text = "↩ Reply",
                Size = new Size(70, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8),
                Cursor = Cursors.Hand
            };
            btnReply.FlatAppearance.BorderSize = 1;
            btnReply.FlatAppearance.BorderColor = Color.LightGray;
            btnReply.Click += (s, e) => ReplyClicked?.Invoke(this, _message!);

            // Forward Button
            btnForward = new Button
            {
                Text = "➤ Forward",
                Size = new Size(75, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8),
                Cursor = Cursors.Hand
            };
            btnForward.FlatAppearance.BorderSize = 1;
            btnForward.FlatAppearance.BorderColor = Color.LightGray;
            btnForward.Click += (s, e) => ForwardClicked?.Invoke(this, _message!);

            // Add controls
            this.Controls.Add(picAvatar);
            this.Controls.Add(pnlBubble);
            this.Controls.Add(btnReply);
            this.Controls.Add(btnForward);

            pnlBubble.Controls.Add(lblUsername);
            pnlBubble.Controls.Add(lblTimestamp);
            pnlBubble.Controls.Add(pnlQuote);
            pnlBubble.Controls.Add(lblContent);
        }

        private void UpdateLayout()
        {
            // Tính toán kích thước bubble
            int quoteHeight = pnlQuote.Visible ? pnlQuote.Height + 5 : 0;
            int contentHeight = lblContent.Height;
            int bubbleHeight = 50 + quoteHeight + contentHeight;

            pnlBubble.Height = bubbleHeight;

            if (_isOwnMessage)
            {
                // Tin nhắn của mình: bên phải, màu xanh
                pnlBubble.BackColor = Color.FromArgb(220, 248, 198);
                pnlBubble.Location = new Point(this.Width - pnlBubble.Width - 60, 10);
                picAvatar.Location = new Point(this.Width - 50, 10);
                
                btnReply.Location = new Point(pnlBubble.Left - 80, pnlBubble.Bottom - 30);
                btnForward.Location = new Point(btnReply.Left - 80, pnlBubble.Bottom - 30);
            }
            else
            {
                // Tin nhắn người khác: bên trái, màu trắng
                pnlBubble.BackColor = Color.White;
                picAvatar.Location = new Point(10, 10);
                pnlBubble.Location = new Point(60, 10);
                
                btnReply.Location = new Point(pnlBubble.Right + 10, pnlBubble.Bottom - 30);
                btnForward.Location = new Point(btnReply.Right + 5, pnlBubble.Bottom - 30);
            }

            // Vị trí các label trong bubble
            lblUsername.Location = new Point(10, 8);
            lblTimestamp.Location = new Point(lblUsername.Right + 10, 10);
            
            int yPos = 28;
            if (pnlQuote.Visible)
            {
                pnlQuote.Location = new Point(10, yPos);
                yPos += pnlQuote.Height + 5;
            }
            
            lblContent.Location = new Point(10, yPos);

            // Border radius
            pnlBubble.Paint += (s, e) =>
            {
                using (GraphicsPath path = GetRoundedRectPath(pnlBubble.ClientRectangle, 15))
                {
                    pnlBubble.Region = new Region(path);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen pen = new Pen(Color.LightGray, 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            this.Height = Math.Max(pnlBubble.Bottom + 20, 80);
        }

        private void LoadAvatar(string? avatarBase64)
        {
            if (!string.IsNullOrEmpty(avatarBase64))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(avatarBase64);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        picAvatar.Image = Image.FromStream(ms);
                    }
                }
                catch
                {
                    SetDefaultAvatar();
                }
            }
            else
            {
                SetDefaultAvatar();
            }

            // Làm tròn avatar
            MakeCircularPictureBox(picAvatar);
        }

        private void SetDefaultAvatar()
        {
            Bitmap bmp = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawString(_message?.SenderUsername?.Substring(0, 1).ToUpper() ?? "?", 
                    new Font("Segoe UI", 16, FontStyle.Bold), 
                    Brushes.White, 
                    new PointF(10, 8));
            }
            picAvatar.Image = bmp;
        }

        private void MakeCircularPictureBox(PictureBox pictureBox)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, pictureBox.Width, pictureBox.Height);
            pictureBox.Region = new Region(path);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
