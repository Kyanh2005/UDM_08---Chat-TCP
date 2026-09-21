namespace ChatApp.Client;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.pnlTopBar = new System.Windows.Forms.Panel();
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblServerIP = new System.Windows.Forms.Label();
        this.txtServerIP = new System.Windows.Forms.TextBox();
        this.lblPort = new System.Windows.Forms.Label();
        this.txtPort = new System.Windows.Forms.TextBox();
        this.lblUsername = new System.Windows.Forms.Label();
        this.txtUsername = new System.Windows.Forms.TextBox();
        this.btnConnect = new System.Windows.Forms.Button();
        this.btnDisconnect = new System.Windows.Forms.Button();
        this.btnAvatar = new System.Windows.Forms.Button();
        this.lblStatus = new System.Windows.Forms.Label();
        this.pnlLeftSidebar = new System.Windows.Forms.Panel();
        this.ucContacts = new ChatApp.Client.UI.Controls.ucContactList();
        this.chatBox = new ChatApp.Client.UI.ChatBoxUserControl();
        this.pnlTopBar.SuspendLayout();
        this.pnlLeftSidebar.SuspendLayout();
        this.SuspendLayout();
        // 
        // pnlTopBar
        // 
        this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
        this.pnlTopBar.Controls.Add(this.lblTitle);
        this.pnlTopBar.Controls.Add(this.lblServerIP);
        this.pnlTopBar.Controls.Add(this.txtServerIP);
        this.pnlTopBar.Controls.Add(this.lblPort);
        this.pnlTopBar.Controls.Add(this.txtPort);
        this.pnlTopBar.Controls.Add(this.lblUsername);
        this.pnlTopBar.Controls.Add(this.txtUsername);
        this.pnlTopBar.Controls.Add(this.btnConnect);
        this.pnlTopBar.Controls.Add(this.btnDisconnect);
        this.pnlTopBar.Controls.Add(this.btnAvatar);
        this.pnlTopBar.Controls.Add(this.lblStatus);
        this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
        this.pnlTopBar.Name = "pnlTopBar";
        this.pnlTopBar.Size = new System.Drawing.Size(1080, 60);
        this.pnlTopBar.TabIndex = 0;
        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(12, 16);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(125, 28);
        this.lblTitle.TabIndex = 0;
        this.lblTitle.Text = "💬 Chat TCP";
        // 
        // lblServerIP
        // 
        this.lblServerIP.AutoSize = true;
        this.lblServerIP.ForeColor = System.Drawing.Color.LightGray;
        this.lblServerIP.Location = new System.Drawing.Point(155, 20);
        this.lblServerIP.Name = "lblServerIP";
        this.lblServerIP.Size = new System.Drawing.Size(24, 20);
        this.lblServerIP.TabIndex = 1;
        this.lblServerIP.Text = "IP:";
        // 
        // txtServerIP
        // 
        this.txtServerIP.Location = new System.Drawing.Point(185, 17);
        this.txtServerIP.Name = "txtServerIP";
        this.txtServerIP.Size = new System.Drawing.Size(100, 27);
        this.txtServerIP.TabIndex = 2;
        this.txtServerIP.Text = "127.0.0.1";
        // 
        // lblPort
        // 
        this.lblPort.AutoSize = true;
        this.lblPort.ForeColor = System.Drawing.Color.LightGray;
        this.lblPort.Location = new System.Drawing.Point(295, 20);
        this.lblPort.Name = "lblPort";
        this.lblPort.Size = new System.Drawing.Size(38, 20);
        this.lblPort.TabIndex = 3;
        this.lblPort.Text = "Port:";
        // 
        // txtPort
        // 
        this.txtPort.Location = new System.Drawing.Point(335, 17);
        this.txtPort.Name = "txtPort";
        this.txtPort.Size = new System.Drawing.Size(55, 27);
        this.txtPort.TabIndex = 4;
        this.txtPort.Text = "5000";
        // 
        // lblUsername
        // 
        this.lblUsername.AutoSize = true;
        this.lblUsername.ForeColor = System.Drawing.Color.LightGray;
        this.lblUsername.Location = new System.Drawing.Point(400, 20);
        this.lblUsername.Name = "lblUsername";
        this.lblUsername.Size = new System.Drawing.Size(35, 20);
        this.lblUsername.TabIndex = 5;
        this.lblUsername.Text = "Tên:";
        // 
        // txtUsername
        // 
        this.txtUsername.Location = new System.Drawing.Point(440, 17);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.PlaceholderText = "Tên người dùng";
        this.txtUsername.Size = new System.Drawing.Size(120, 27);
        this.txtUsername.TabIndex = 6;
        // 
        // btnConnect
        // 
        this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
        this.btnConnect.FlatAppearance.BorderSize = 0;
        this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnConnect.ForeColor = System.Drawing.Color.White;
        this.btnConnect.Location = new System.Drawing.Point(570, 15);
        this.btnConnect.Name = "btnConnect";
        this.btnConnect.Size = new System.Drawing.Size(80, 30);
        this.btnConnect.TabIndex = 7;
        this.btnConnect.Text = "Kết nối";
        this.btnConnect.UseVisualStyleBackColor = false;
        // 
        // btnDisconnect
        // 
        this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
        this.btnDisconnect.Enabled = false;
        this.btnDisconnect.FlatAppearance.BorderSize = 0;
        this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnDisconnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnDisconnect.ForeColor = System.Drawing.Color.White;
        this.btnDisconnect.Location = new System.Drawing.Point(655, 15);
        this.btnDisconnect.Name = "btnDisconnect";
        this.btnDisconnect.Size = new System.Drawing.Size(70, 30);
        this.btnDisconnect.TabIndex = 8;
        this.btnDisconnect.Text = "Ngắt";
        this.btnDisconnect.UseVisualStyleBackColor = false;
        // 
        // btnAvatar
        // 
        this.btnAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
        this.btnAvatar.FlatAppearance.BorderSize = 0;
        this.btnAvatar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnAvatar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnAvatar.ForeColor = System.Drawing.Color.White;
        this.btnAvatar.Location = new System.Drawing.Point(735, 15);
        this.btnAvatar.Name = "btnAvatar";
        this.btnAvatar.Size = new System.Drawing.Size(85, 30);
        this.btnAvatar.TabIndex = 9;
        this.btnAvatar.Text = "👤 Avatar";
        this.btnAvatar.UseVisualStyleBackColor = false;
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
        this.lblStatus.Location = new System.Drawing.Point(835, 20);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(123, 20);
        this.lblStatus.TabIndex = 10;
        this.lblStatus.Text = "🔴 Chưa kết nối";
        // 
        // pnlLeftSidebar
        // 
        this.pnlLeftSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
        this.pnlLeftSidebar.Controls.Add(this.ucContacts);
        this.pnlLeftSidebar.Dock = System.Windows.Forms.DockStyle.Left;
        this.pnlLeftSidebar.Location = new System.Drawing.Point(0, 60);
        this.pnlLeftSidebar.Name = "pnlLeftSidebar";
        this.pnlLeftSidebar.Size = new System.Drawing.Size(260, 590);
        this.pnlLeftSidebar.TabIndex = 1;
        // 
        // ucContacts
        // 
        this.ucContacts.Dock = System.Windows.Forms.DockStyle.Fill;
        this.ucContacts.Location = new System.Drawing.Point(0, 0);
        this.ucContacts.Name = "ucContacts";
        this.ucContacts.Size = new System.Drawing.Size(260, 590);
        this.ucContacts.TabIndex = 0;
        // 
        // chatBox
        // 
        this.chatBox.BackColor = System.Drawing.Color.White;
        this.chatBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.chatBox.Location = new System.Drawing.Point(260, 60);
        this.chatBox.Name = "chatBox";
        this.chatBox.Size = new System.Drawing.Size(820, 590);
        this.chatBox.TabIndex = 2;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1080, 650);
        this.Controls.Add(this.chatBox);
        this.Controls.Add(this.pnlLeftSidebar);
        this.Controls.Add(this.pnlTopBar);
        this.MinimumSize = new System.Drawing.Size(950, 600);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "TCP Chat Application - Nhóm 8";
        this.pnlTopBar.ResumeLayout(false);
        this.pnlTopBar.PerformLayout();
        this.pnlLeftSidebar.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlTopBar;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblServerIP;
    private System.Windows.Forms.TextBox txtServerIP;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.TextBox txtPort;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.Button btnDisconnect;
    private System.Windows.Forms.Button btnAvatar;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Panel pnlLeftSidebar;
    private ChatApp.Client.UI.Controls.ucContactList ucContacts;
    private ChatApp.Client.UI.ChatBoxUserControl chatBox;
}
