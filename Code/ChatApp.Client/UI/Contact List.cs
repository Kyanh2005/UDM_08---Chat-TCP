```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatApp.Client
{
    public class UCContactList : UserControl
    {
        private Panel pnlHeader;
        private TextBox txtSearch;
        private FlowLayoutPanel flpContacts;

        public event EventHandler<ContactEventArgs> ContactSelected;

        public UCContactList()
        {
            InitializeComponent();

            // Dữ liệu mẫu để test
            AddContact("Nguyễn Văn A", "Online");
            AddContact("Trần Văn B", "Offline");
            AddContact("Lê Minh C", "Online");
            AddContact("Phạm Văn D", "Offline");
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // ===== HEADER =====
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = Color.FromArgb(30, 144, 255);

            Label lblTitle = new Label();
            lblTitle.Text = "Danh bạ";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font(
                "Segoe UI",
                16,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(15, 8);
            lblTitle.AutoSize = true;

            txtSearch = new TextBox();
            txtSearch.PlaceholderText = "Tìm kiếm liên hệ...";
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(15, 38);
            txtSearch.Width = 270;

            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(txtSearch);

            // ===== CONTACT LIST =====
            flpContacts = new FlowLayoutPanel();
            flpContacts.Dock = DockStyle.Fill;
            flpContacts.FlowDirection = FlowDirection.TopDown;
            flpContacts.WrapContents = false;
            flpContacts.AutoScroll = true;
            flpContacts.BackColor = Color.White;
            flpContacts.Padding = new Padding(5);

            this.Controls.Add(flpContacts);
            this.Controls.Add(pnlHeader);
        }

        // ===== THÊM CONTACT =====
        public void AddContact(string username, string status)
        {
            Panel contact = new Panel();

            contact.Width = 300;
            contact.Height = 65;
            contact.BackColor = Color.White;
            contact.Margin = new Padding(3);
            contact.Cursor = Cursors.Hand;

            // Avatar
            Label avatar = new Label();

            avatar.Text = username.Length > 0
                ? username.Substring(0, 1).ToUpper()
                : "?";

            avatar.TextAlign = ContentAlignment.MiddleCenter;
            avatar.Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold
            );
            avatar.ForeColor = Color.White;
            avatar.BackColor = Color.FromArgb(70, 130, 180);
            avatar.Location = new Point(8, 8);
            avatar.Size = new Size(45, 45);

            // Tên
            Label lblName = new Label();
            lblName.Text = username;
            lblName.Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            );
            lblName.Location = new Point(65, 10);
            lblName.AutoSize = true;

            // Trạng thái
            Label lblStatus = new Label();
            lblStatus.Text = status;
            lblStatus.Font = new Font(
                "Segoe UI",
                9
            );

            lblStatus.ForeColor =
                status == "Online"
                ? Color.Green
                : Color.Gray;

            lblStatus.Location = new Point(65, 35);
            lblStatus.AutoSize = true;

            // Click contact
            contact.Click += (s, e) =>
            {
                ContactSelected?.Invoke(
                    this,
                    new ContactEventArgs(username)
                );
            };

            avatar.Click += (s, e) =>
            {
                ContactSelected?.Invoke(
                    this,
                    new ContactEventArgs(username)
                );
            };

            lblName.Click += (s, e) =>
            {
                ContactSelected?.Invoke(
                    this,
                    new ContactEventArgs(username)
                );
            };

            lblStatus.Click += (s, e) =>
            {
                ContactSelected?.Invoke(
                    this,
                    new ContactEventArgs(username)
                );
            };

            contact.Controls.Add(avatar);
            contact.Controls.Add(lblName);
            contact.Controls.Add(lblStatus);

            flpContacts.Controls.Add(contact);
        }

        // ===== TÌM KIẾM =====
        private void TxtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            string keyword =
                txtSearch.Text.Trim().ToLower();

            foreach (Control control in flpContacts.Controls)
            {
                if (control.Controls.Count < 2)
                    continue;

                Label lblName =
                    control.Controls[1] as Label;

                if (lblName != null)
                {
                    control.Visible =
                        lblName.Text
                            .ToLower()
                            .Contains(keyword);
                }
            }
        }
    }

    // ===== EVENT CONTACT =====
    public class ContactEventArgs : EventArgs
    {
        public string Username { get; }

        public ContactEventArgs(string username)
        {
            Username = username;
        }
    }
}
```
