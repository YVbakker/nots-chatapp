
namespace ChatClient;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        chatBox.IsReadOnly = true;
        chatBox.Text = "Welcome to ChatClient, press 'connect' to get started.\n";
        btnSend.IsEnabled = false;
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        MauiProgram.Client = new Client(int.Parse(txtPort.Text), txtIPServer.Text, Chat);
        await MauiProgram.Client.Start();
        if (!MauiProgram.Client.Connected()) return;
        btnConnect.IsEnabled = false;
        btnConnect.BackgroundColor = Color.FromRgb(255, 154, 154);
        btnConnect.Text = "Disconnect";
    }

    private void txtMessage_TextChanged(object sender, EventArgs e)
    {
        btnSend.IsEnabled = txtMessage.Text.Length > 0;
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        btnSend.IsEnabled = false;
        await MauiProgram.Client.SendAsync(txtMessage.Text);
        txtMessage.Text = string.Empty;
    }

    public void txtMessage_Completed(object sender, EventArgs e)
    {
        btnSend_Click(sender, e);
        txtMessage.Focus();
    }

    public void Chat(string sender, string message)
    {
        chatBox.Text += $"[{sender}] {message}\n";
    }
}