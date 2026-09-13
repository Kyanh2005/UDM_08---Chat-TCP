namespace ChatApp.Client.UI
{
    partial class ChatBoxUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblChatTitle = new System.Windows.Forms.Label();
            this.pnlMessages = new System.Windows.Forms.Panel();
            this.flowLayoutPanelMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlInputArea = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.pnlHeader.SuspendLayout();
            this.pnlMessages.SuspendLayout();
            this.pnlInputArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.pnlHeader.Controls.Add(this.lblChatTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(600, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblChatTitle
            // 
            this.lblChatTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblChatTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblChatTitle.ForeColor = System.Drawing.Color.White;
            this.lblChatTitle.Location = new System.Drawing.Point(0, 0);
            this.lblChatTitle.Name = "lblChatTitle";
            this.lblChatTitle.Size = new System.Drawing.Size(600, 50);
            this.lblChatTitle.TabIndex = 0;
            this.lblChatTitle.Text = "Chat Window";
            this.lblChatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblChatTitle.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            // 
            // pnlMessages
            // 
            this.pnlMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlMessages.Controls.Add(this.flowLayoutPanelMessages);
            this.pnlMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessages.Location = new System.Drawing.Point(0, 50);
            this.pnlMessages.Name = "pnlMessages";
            this.pnlMessages.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMessages.Size = new System.Drawing.Size(600, 370);
            this.pnlMessages.TabIndex = 1;
            // 
            // flowLayoutPanelMessages
            // 
            this.flowLayoutPanelMessages.AutoScroll = true;
            this.flowLayoutPanelMessages.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanelMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelMessages.Location = new System.Drawing.Point(10, 10);
            this.flowLayoutPanelMessages.Name = "flowLayoutPanelMessages";
            this.flowLayoutPanelMessages.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanelMessages.Size = new System.Drawing.Size(580, 350);
            this.flowLayoutPanelMessages.TabIndex = 0;
            this.flowLayoutPanelMessages.WrapContents = false;
            // 
            // pnlInputArea
            // 
            this.pnlInputArea.BackColor = System.Drawing.Color.White;
            this.pnlInputArea.Controls.Add(this.btnSend);
            this.pnlInputArea.Controls.Add(this.txtMessage);
            this.pnlInputArea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInputArea.Location = new System.Drawing.Point(0, 420);
            this.pnlInputArea.Name = "pnlInputArea";
            this.pnlInputArea.Padding = new System.Windows.Forms.Padding(10);
            this.pnlInputArea.Size = new System.Drawing.Size(600, 80);
            this.pnlInputArea.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(490, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 60);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMessage.Location = new System.Drawing.Point(10, 10);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.PlaceholderText = "Type a message...";
            this.txtMessage.Size = new System.Drawing.Size(580, 60);
            this.txtMessage.TabIndex = 0;
            // 
            // ChatBoxUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlMessages);
            this.Controls.Add(this.pnlInputArea);
            this.Controls.Add(this.pnlHeader);
            this.Name = "ChatBoxUserControl";
            this.Size = new System.Drawing.Size(600, 500);
            this.pnlHeader.ResumeLayout(false);
            this.pnlMessages.ResumeLayout(false);
            this.pnlInputArea.ResumeLayout(false);
            this.pnlInputArea.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblChatTitle;
        private System.Windows.Forms.Panel pnlMessages;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMessages;
        private System.Windows.Forms.Panel pnlInputArea;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessage;
    }
}
