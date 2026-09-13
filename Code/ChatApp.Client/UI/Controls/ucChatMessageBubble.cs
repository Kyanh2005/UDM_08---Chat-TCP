using System;
using System.Windows.Forms;

namespace ChatApp.Client.UI.Controls
{
    /// <summary>
    /// [GIAI ĐOẠN 1] UserControl rỗng - Member 5
    /// Bong bóng tin nhắn (sẽ hoàn thiện ở giai đoạn 2-3)
    /// </summary>
    public partial class ucChatMessageBubble : UserControl
    {
        public ucChatMessageBubble()
        {
            InitializeComponent();
            SetupEmptyUI();
        }

        private void SetupEmptyUI()
        {
            // Placeholder cho giai đoạn 1
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Size = new System.Drawing.Size(400, 80);
            this.BorderStyle = BorderStyle.FixedSingle;

            Label lblPlaceholder = new Label
            {
                Text = "[ucChatMessageBubble] - Member 5\nChờ giai đoạn 2 để hoàn thiện",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic)
            };

            this.Controls.Add(lblPlaceholder);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "ucChatMessageBubble";
            this.ResumeLayout(false);
        }
    }
}
