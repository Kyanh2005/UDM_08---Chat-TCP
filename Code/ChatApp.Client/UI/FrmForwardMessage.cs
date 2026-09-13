using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChatApp.Shared.Models;

namespace ChatApp.Client.UI
{
    /// <summary>
    /// Form popup để chọn người nhận khi Forward tin nhắn - Member 5
    /// </summary>
    public partial class FrmForwardMessage : Form
    {
        public string? SelectedUsername { get; private set; }
        private List<string> _onlineUsers;
        private ListBox listUsers;

        public FrmForwardMessage(List<string> onlineUsers, string currentUsername)
        {
            InitializeComponent();
            _onlineUsers = onlineUsers.Where(u => u != currentUsername).ToList();
            SetupUI();
            LoadUsers();
        }

        private void SetupUI()
        {
            this.Text = "Chuyển tiếp tin nhắn";
            this.Size = new Size(350, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            Label lblTitle = new Label
            {
                Text = "➤ Chọn người nhận",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Search box
            Panel pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(15, 10, 15, 10)
            };

            TextBox txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "🔍 Tìm kiếm..."
            };
            txtSearch.TextChanged += (s, e) => FilterUsers(txtSearch.Text);

            pnlSearch.Controls.Add(txtSearch);

            // ListBox hiển thị danh sách user
            listUsers = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                ItemHeight = 35,
                DrawMode = DrawMode.OwnerDrawFixed
            };
            listUsers.DrawItem += ListUsers_DrawItem;
            listUsers.DoubleClick += (s, e) => BtnOK_Click(s!, e);

            // Buttons
            Panel pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(15, 10, 15, 10)
            };

            Button btnOK = new Button
            {
                Text = "✓ Chuyển tiếp",
                Dock = DockStyle.Right,
                Width = 120,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 200, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            Button btnCancel = new Button
            {
                Text = "✕ Hủy",
                Dock = DockStyle.Right,
                Width = 80,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 220),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);

            this.Controls.Add(listUsers);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlButtons);
        }

        private void LoadUsers()
        {
            listUsers.Items.Clear();
            foreach (string user in _onlineUsers)
            {
                listUsers.Items.Add(user);
            }

            if (listUsers.Items.Count > 0)
            {
                listUsers.SelectedIndex = 0;
            }
        }

        private void FilterUsers(string searchText)
        {
            listUsers.Items.Clear();
            var filtered = _onlineUsers.Where(u => 
                u.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (string user in filtered)
            {
                listUsers.Items.Add(user);
            }
        }

        private void ListUsers_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            string username = listUsers.Items[e.Index].ToString()!;
            
            // Background màu khi selected
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 240, 255)), e.Bounds);
            }

            // Vẽ avatar tròn
            int avatarSize = 28;
            Rectangle avatarRect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 4, avatarSize, avatarSize);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillEllipse(new SolidBrush(Color.LightGray), avatarRect);
            
            // Chữ cái đầu
            string initial = username.Substring(0, 1).ToUpper();
            e.Graphics.DrawString(initial, 
                new Font("Segoe UI", 10, FontStyle.Bold), 
                Brushes.White, 
                avatarRect.X + 8, 
                avatarRect.Y + 6);

            // Username
            e.Graphics.DrawString(username, 
                new Font("Segoe UI", 10), 
                Brushes.Black, 
                avatarRect.Right + 10, 
                e.Bounds.Y + 8);

            e.DrawFocusRectangle();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (listUsers.SelectedItem != null)
            {
                SelectedUsername = listUsers.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người nhận!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
