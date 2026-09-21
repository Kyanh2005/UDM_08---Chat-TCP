using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChatApp.Shared.Models;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// UserControl hiển thị danh sách người dùng Online/Offline và Tìm kiếm liên hệ - Member 4
    /// </summary>
    public class ucContactList : UserControl
    {
        public event EventHandler<string>? ContactSelected;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private TextBox txtSearch;
        private FlowLayoutPanel flpContacts;
        private ucContactItem itemAllRoom = null!;
        private readonly Dictionary<string, ucContactItem> _contactItems = new(StringComparer.OrdinalIgnoreCase);
        private string _selectedReceiver = "ALL";
        private string _myUsername = "";

        public string SelectedReceiver => _selectedReceiver;

        public ucContactList()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Size = new Size(260, 500);
            this.BackColor = Color.FromArgb(249, 250, 251);

            // ===== HEADER PANEL =====
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(243, 244, 246),
                Padding = new Padding(10, 8, 10, 8)
            };

            lblHeaderTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                Text = "DANH BẠ TRỰC TUYẾN [0]",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 85, 99),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                Font = new Font("Segoe UI", 9f),
                PlaceholderText = "🔍 Tìm bạn bè theo tên..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlHeader.Controls.Add(txtSearch);
            pnlHeader.Controls.Add(lblHeaderTitle);

            // ===== CONTACT LIST (SCROLLABLE) =====
            flpContacts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(249, 250, 251),
                Padding = new Padding(5, 5, 5, 5)
            };

            this.Controls.Add(flpContacts);
            this.Controls.Add(pnlHeader);

            // Khởi tạo mục phòng chung ALL
            InitAllRoomItem();
        }

        private void InitAllRoomItem()
        {
            itemAllRoom = new ucContactItem();
            itemAllRoom.SetData("ALL", "Phòng chat chung (ALL)", null, isOnline: true, isAllRoom: true);
            itemAllRoom.IsSelected = true;
            itemAllRoom.ContactClicked += (s, username) => SelectContact("ALL");

            flpContacts.Controls.Add(itemAllRoom);
        }

        public void SetMyUsername(string myUsername)
        {
            _myUsername = myUsername;
        }

        public void SetOnlineUsers(List<User> users, string myUsername)
        {
            _myUsername = myUsername;

            // Xóa các user cũ (giữ lại phòng ALL)
            foreach (var item in _contactItems.Values)
            {
                flpContacts.Controls.Remove(item);
                item.Dispose();
            }
            _contactItems.Clear();

            foreach (var user in users)
            {
                if (user.Username.Equals(myUsername, StringComparison.OrdinalIgnoreCase)) continue;

                var item = new ucContactItem();
                item.SetData(user.Username, user.DisplayName, user.AvatarBase64, user.IsOnline);
                item.ContactClicked += (s, targetUser) => SelectContact(targetUser);

                _contactItems[user.Username] = item;
                flpContacts.Controls.Add(item);
            }

            UpdateHeaderCount();
            ApplyFilter();
        }

        public void AddOrUpdateUser(string username, string? avatarBase64, bool isOnline)
        {
            if (username.Equals(_myUsername, StringComparison.OrdinalIgnoreCase)) return;

            if (_contactItems.TryGetValue(username, out var existingItem))
            {
                existingItem.UpdateStatus(isOnline);
                if (!string.IsNullOrEmpty(avatarBase64))
                {
                    existingItem.UpdateAvatar(avatarBase64);
                }
            }
            else
            {
                var item = new ucContactItem();
                item.SetData(username, username, avatarBase64, isOnline);
                item.ContactClicked += (s, targetUser) => SelectContact(targetUser);

                _contactItems[username] = item;
                flpContacts.Controls.Add(item);
            }

            UpdateHeaderCount();
            ApplyFilter();
        }

        public void RemoveUser(string username)
        {
            if (_contactItems.TryGetValue(username, out var item))
            {
                flpContacts.Controls.Remove(item);
                item.Dispose();
                _contactItems.Remove(username);

                if (_selectedReceiver.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    SelectContact("ALL");
                }
            }

            UpdateHeaderCount();
        }

        public void UpdateUserAvatar(string username, string? avatarBase64)
        {
            if (_contactItems.TryGetValue(username, out var item))
            {
                item.UpdateAvatar(avatarBase64);
            }
        }

        public void ClearUsers()
        {
            foreach (var item in _contactItems.Values)
            {
                flpContacts.Controls.Remove(item);
                item.Dispose();
            }
            _contactItems.Clear();
            SelectContact("ALL");
            UpdateHeaderCount();
        }

        private void SelectContact(string username)
        {
            _selectedReceiver = username;

            itemAllRoom.IsSelected = username.Equals("ALL", StringComparison.OrdinalIgnoreCase);

            foreach (var kvp in _contactItems)
            {
                kvp.Value.IsSelected = kvp.Key.Equals(username, StringComparison.OrdinalIgnoreCase);
            }

            ContactSelected?.Invoke(this, _selectedReceiver);
        }

        private void UpdateHeaderCount()
        {
            int onlineCount = _contactItems.Values.Count(i => i.IsOnline);
            lblHeaderTitle.Text = $"DANH BẠ TRỰC TUYẾN [{onlineCount}]";
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            foreach (var kvp in _contactItems)
            {
                bool isMatch = string.IsNullOrEmpty(keyword) || kvp.Key.ToLower().Contains(keyword);
                kvp.Value.Visible = isMatch;
            }
        }
    }
}
