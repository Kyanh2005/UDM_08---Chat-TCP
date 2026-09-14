using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// UserControl đại diện cho 1 dòng liên hệ trong danh bạ - Member 4
    /// Hỗ trợ: Avatar tròn/bo góc, tên người dùng, trạng thái Online/Offline, hiệu ứng hover/click.
    /// </summary>
    public class ucContactItem : UserControl
    {
        public event EventHandler<string>? ContactClicked;

        private PictureBox picAvatar;
        private Label lblName;
        private Label lblStatus;
        private Panel pnlStatusDot;
        private bool _isSelected;
        private bool _isOnline = true;
        private string _username = "";
        private string? _avatarBase64;

        public string Username => _username;
        public bool IsOnline => _isOnline;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateBackground();
            }
        }

        public ucContactItem()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Size = new Size(240, 56);
            this.Margin = new Padding(2, 2, 2, 2);
            this.Cursor = Cursors.Hand;
            this.BackColor = Color.White;

            // Avatar tròn
            picAvatar = new PictureBox
            {
                Size = new Size(38, 38),
                Location = new Point(8, 9),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(220, 230, 242)
            };
            picAvatar.Paint += PicAvatar_Paint;

            // Chấm trạng thái Online (xanh lá) / Offline (xám)
            pnlStatusDot = new Panel
            {
                Size = new Size(10, 10),
                Location = new Point(36, 37),
                BackColor = Color.FromArgb(16, 185, 129)
            };
            pnlStatusDot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(pnlStatusDot.BackColor);
                e.Graphics.FillEllipse(brush, 0, 0, 9, 9);
            };

            // Tên hiển thị
            lblName = new Label
            {
                Location = new Point(54, 10),
                Size = new Size(175, 18),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoEllipsis = true
            };

            // Dòng trạng thái (Online / Offline)
            lblStatus = new Label
            {
                Location = new Point(54, 28),
                Size = new Size(175, 16),
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.Gray,
                AutoEllipsis = true
            };

            this.Controls.Add(pnlStatusDot);
            this.Controls.Add(picAvatar);
            this.Controls.Add(lblName);
            this.Controls.Add(lblStatus);

            pnlStatusDot.BringToFront();

            // Gán sự kiện Click cho mọi thành phần con
            this.Click += Item_Clicked;
            picAvatar.Click += Item_Clicked;
            lblName.Click += Item_Clicked;
            lblStatus.Click += Item_Clicked;

            // Hiệu ứng Hover chuột
            this.MouseEnter += (s, e) => { if (!_isSelected) this.BackColor = Color.FromArgb(243, 244, 246); };
            this.MouseLeave += (s, e) => { if (!_isSelected) this.BackColor = Color.White; };
        }

        private void Item_Clicked(object? sender, EventArgs e)
        {
            ContactClicked?.Invoke(this, _username);
        }

        public void SetData(string username, string displayName, string? avatarBase64, bool isOnline, bool isAllRoom = false)
        {
            _username = username;
            _avatarBase64 = avatarBase64;
            _isOnline = isOnline;

            lblName.Text = isAllRoom ? "📢 " + displayName : displayName;
            lblStatus.Text = isAllRoom ? "Tất cả thành viên" : (isOnline ? "Đang trực tuyến" : "Ngoại tuyến");

            pnlStatusDot.BackColor = isOnline ? Color.FromArgb(16, 185, 129) : Color.LightGray;
            pnlStatusDot.Visible = !isAllRoom;

            LoadAvatar(avatarBase64, displayName, isAllRoom);
            UpdateBackground();
        }

        public void UpdateAvatar(string? avatarBase64)
        {
            _avatarBase64 = avatarBase64;
            LoadAvatar(avatarBase64, lblName.Text, _username == "ALL");
        }

        public void UpdateStatus(bool isOnline)
        {
            _isOnline = isOnline;
            lblStatus.Text = isOnline ? "Đang trực tuyến" : "Ngoại tuyến";
            pnlStatusDot.BackColor = isOnline ? Color.FromArgb(16, 185, 129) : Color.LightGray;
        }

        private void LoadAvatar(string? base64, string name, bool isAllRoom)
        {
            if (isAllRoom)
            {
                picAvatar.Image = null;
                picAvatar.BackColor = Color.FromArgb(59, 130, 246); // Xanh dương cho phòng chung
                return;
            }

            if (!string.IsNullOrEmpty(base64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    using var ms = new MemoryStream(bytes);
                    picAvatar.Image = Image.FromStream(ms);
                    return;
                }
                catch { }
            }

            picAvatar.Image = null;
            picAvatar.BackColor = Color.FromArgb(99, 102, 241); // Màu tím indigo mặc định
        }

        private void PicAvatar_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Bo tròn Avatar
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, picAvatar.Width - 1, picAvatar.Height - 1);
                picAvatar.Region = new Region(path);

                // Viền tròn
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1.5f);
                e.Graphics.DrawEllipse(pen, 0, 0, picAvatar.Width - 1, picAvatar.Height - 1);
            }

            // Nếu không có ảnh, vẽ chữ cái đầu làm Avatar
            if (picAvatar.Image == null)
            {
                string initial = _username == "ALL" ? "📢" : (_username.Length > 0 ? _username.Substring(0, 1).ToUpper() : "?");
                using var font = new Font("Segoe UI", 12, FontStyle.Bold);
                using var brush = new SolidBrush(Color.White);
                var size = e.Graphics.MeasureString(initial, font);
                float x = (picAvatar.Width - size.Width) / 2;
                float y = (picAvatar.Height - size.Height) / 2;
                e.Graphics.DrawString(initial, font, brush, x, y);
            }
        }

        private void UpdateBackground()
        {
            if (_isSelected)
            {
                this.BackColor = Color.FromArgb(224, 242, 254); // Xanh nhạt khi chọn
                lblName.ForeColor = Color.FromArgb(2, 132, 199);
            }
            else
            {
                this.BackColor = Color.White;
                lblName.ForeColor = Color.FromArgb(33, 37, 41);
            }
        }
    }
}
