using System;
using System.Windows.Forms;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// [GIAI ĐOẠN 1] UserControl rỗng - Member 5
    /// Emoji Picker (sẽ hoàn thiện ở giai đoạn 3)
    /// </summary>
    public partial class ucEmojiPicker : UserControl
    {
        public ucEmojiPicker()
        {
            InitializeComponent();
            SetupEmptyUI();
        }

        private void SetupEmptyUI()
        {
            // Placeholder cho giai đoạn 1
            this.BackColor = System.Drawing.Color.LightYellow;
            this.Size = new System.Drawing.Size(300, 200);
            this.BorderStyle = BorderStyle.FixedSingle;

            Label lblPlaceholder = new Label
            {
                Text = "[ucEmojiPicker] - Member 5\n😀 Emoji Picker\nChờ giai đoạn 3 để hoàn thiện",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic)
            };

            this.Controls.Add(lblPlaceholder);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "ucEmojiPicker";
            this.ResumeLayout(false);
        }
    }
}
