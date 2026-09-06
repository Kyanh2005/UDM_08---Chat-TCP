using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Client.Network
{
    public class FormMain : Form1
    {
        private readonly TcpClientService _netService;
    private readonly Progress<string> _dataProgress;
    private readonly Progress<(bool IsConnected, string Message)> _statusProgress;

    // Giả định UserControl từ Mem 4 & Mem 5
    private UserControlMem4 _ucMem4;
    private UserControlMem5 _ucMem5;

    public FrmMain()
    {
        InitializeComponent();

        _netService = new TcpClientService();

        // Sử dụng Progress<T> để tự động Invoke về UI Thread an toàn
        _dataProgress = new Progress<string>(HandleDataReceived);
        _statusProgress = new Progress<(bool, string)>(HandleStatusChanged);

        // Đăng ký nhận sự kiện từ Network Service
        _netService.OnDataReceived += data => ((IProgress<string>)_dataProgress).Report(data);
        _netService.OnConnectionStatusChanged += (status, msg) => 
            ((IProgress<(bool, string)>)_statusProgress).Report((status, msg));

        InitUserControls();
    }

    private void InitUserControls()
    {
        // Ghép nối UserControl Mem 4 và Mem 5 vào các Panel trên FrmMain
        _ucMem4 = new UserControlMem4 { Dock = DockStyle.Fill };
        _ucMem5 = new UserControlMem5 { Dock = DockStyle.Fill };

        panelMem4Container.Controls.Add(_ucMem4);
        panelMem5Container.Controls.Add(_ucMem5);
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        btnConnect.Enabled = false;
        await _netService.ConnectAsync(txtIp.Text, int.Parse(txtPort.Text));
        btnConnect.Enabled = !_netService.IsConnected;
    }

    private void HandleDataReceived(string data)
    {
        // Hàm này LUÔN CHẠY TRÊN UI THREAD nhờ Progress<T>
        // Truyền dữ liệu giải mã cho UserControl Mem 4 / Mem 5 xử lý
        _ucMem4.UpdateNetworkData(data);
        _ucMem5.ProcessIncomingMessage(data);
    }

    private void HandleStatusChanged((bool IsConnected, string Message) status)
    {
        // Cập nhật trạng thái kết nối lên UI
        lblStatus.Text = status.Message;
        lblStatus.ForeColor = status.IsConnected ? Color.Green : Color.Red;
        btnConnect.Enabled = !status.IsConnected;

        if (!status.IsConnected)
        {
            _ucMem4.OnNetworkDisconnected();
            _ucMem5.OnNetworkDisconnected();
        }
    }

    private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        _netService.Disconnect();
    }
    }
}