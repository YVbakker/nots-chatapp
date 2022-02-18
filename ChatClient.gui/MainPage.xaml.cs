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
        int safePort = 9000;
        int safeBufferSize = 1024;
        var isPortParsed = int.TryParse(txtPort.Text, out safePort);
        var isBufferSizeParsed = int.TryParse(txtBufferSize.Text, out safeBufferSize);
        if (isPortParsed && isBufferSizeParsed)
        {
            MauiProgram.Client = new Client(txtIPServer.Text, safePort, safeBufferSize, Chat);
        }
        else
        {
            Chat("client", "Please enter a valid port number and/or buffer size");
            return;
        }

        try
        {
            await MauiProgram.Client.Connect();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }

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
        await MauiProgram.Client.SendMessageAsync(txtMessage.Text);
        txtMessage.Text = string.Empty;
    }

    public void txtMessage_Completed(object sender, EventArgs e)
    {
        btnSend_Click(sender, e);
    }

    public void Chat(string sender, string message)
    {
        chatBox.Text += $"[{sender}] {message}\n";
    }
}