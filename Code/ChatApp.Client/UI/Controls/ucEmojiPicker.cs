using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// UserControl Emoji Picker - Member 5
    /// Popup chọn emoji Unicode để chèn vào TextBox
    /// </summary>
    public partial class ucEmojiPicker : UserControl
    {
        // Event khi chọn emoji
        public event EventHandler<string>? EmojiSelected;

        private FlowLayoutPanel flowPanel;

        // Danh sách emoji phổ biến
        private readonly string[] _emojis = new string[]
        {
            // Mặt cười
            "😀", "😃", "😄", "😁", "😆", "😅", "🤣", "😂",
            "🙂", "🙃", "😉", "😊", "😇", "🥰", "😍", "🤩",
            "😘", "😗", "😚", "😙", "😋", "😛", "😜", "🤪",
            "😝", "🤑", "🤗", "🤭", "🤫", "🤔", "🤐", "🤨",
            
            // Cảm xúc
            "😐", "😑", "😶", "😏", "😒", "🙄", "😬", "🤥",
            "😌", "😔", "😪", "🤤", "😴", "😷", "🤒", "🤕",
            "🤢", "🤮", "🤧", "🥵", "🥶", "😵", "🤯", "🤠",
            "🥳", "😎", "🤓", "🧐", "😕", "😟", "🙁", "😮",
            
            // Cử chỉ tay
            "👍", "👎", "👌", "✌", "🤞", "🤟", "🤘", "🤙",
            "👈", "👉", "👆", "👇", "☝", "✋", "🤚", "🖐",
            "🖖", "👋", "🤝", "💪", "🙏", "✍", "💅", "🤳",
            
            // Tim và biểu tượng
            "❤", "🧡", "💛", "💚", "💙", "💜", "🖤", "🤍",
            "💔", "❣", "💕", "💞", "💓", "💗", "💖", "💘",
            "💝", "💟", "☮", "✝", "☪", "🕉", "☸", "✡",
            
            // Động vật
            "🐶", "🐱", "🐭", "🐹", "🐰", "🦊", "🐻", "🐼",
            "🐨", "🐯", "🦁", "🐮", "🐷", "🐸", "🐵", "🐔",
            "🐧", "🐦", "🐤", "🦆", "🦅", "🦉", "🦇", "🐺",
            
            // Thức ăn
            "🍏", "🍎", "🍐", "🍊", "🍋", "🍌", "🍉", "🍇",
            "🍓", "🍈", "🍒", "🍑", "🥭", "🍍", "🥥", "🥝",
            "🍅", "🍆", "🥑", "🥦", "🥬", "🥒", "🌶", "🌽",
            "🍔", "🍟", "🍕", "🌭", "🥪", "🌮", "🌯", "🥙",
            
            // Hoạt động
            "⚽", "🏀", "🏈", "⚾", "🥎", "🎾", "🏐", "🏉",
            "🥏", "🎱", "🏓", "🏸", "🏒", "🏑", "🥍", "🏏",
            "⛳", "🏹", "🎣", "🥊", "🥋", "🎽", "⛸", "🥌"
        };

        public ucEmojiPicker()
        {
            InitializeComponent();
            SetupUI();
            LoadEmojis();
        }

        private void SetupUI()
        {
            this.Size = new Size(320, 280);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.AutoScroll = false;

            // Header
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Label lblTitle = new Label
            {
                Text = "😊 Chọn Emoji",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            Button btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 35,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Visible = false;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);

            // Flow Panel chứa emoji
            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5),
                WrapContents = true
            };

            this.Controls.Add(flowPanel);
            this.Controls.Add(pnlHeader);
        }

        private void LoadEmojis()
        {
            foreach (string emoji in _emojis)
            {
                Button btnEmoji = new Button
                {
                    Text = emoji,
                    Size = new Size(35, 35),
                    Font = new Font("Segoe UI Emoji", 16),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(2),
                    Tag = emoji
                };

                btnEmoji.FlatAppearance.BorderSize = 0;
                btnEmoji.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);

                btnEmoji.Click += (s, e) =>
                {
                    string selectedEmoji = ((Button)s!).Tag.ToString()!;
                    EmojiSelected?.Invoke(this, selectedEmoji);
                    this.Visible = false; // Đóng popup sau khi chọn
                };

                // Tooltip
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btnEmoji, emoji);

                flowPanel.Controls.Add(btnEmoji);
            }
        }

        /// <summary>
        /// Hiển thị Emoji Picker tại vị trí cụ thể
        /// </summary>
        public void ShowAt(Control parent, Point location)
        {
            this.Location = location;
            this.Visible = true;
            this.BringToFront();
        }
    }
}
