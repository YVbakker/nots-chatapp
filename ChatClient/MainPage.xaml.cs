using System.Collections.ObjectModel;

namespace ChatClient;

public partial class MainPage : ContentPage
{
    public List<string> chatbox = new List<string>();
    public MainPage()
    {
        InitializeComponent();
        listChats.Header = "Messages";
    }

    private IEnumerable<string> Chat(string message)
    {
        chatbox.Add(message);
        return chatbox;
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        MauiProgram.Client = new Client(int.Parse(txtPort.Text), txtIPServer.Text);
        await MauiProgram.Client.Start();
        if (MauiProgram.Client.Connected())
        {
            btnConnect.IsEnabled = false;
            btnConnect.BackgroundColor = Color.FromRgb(255, 154, 154);
            btnConnect.Text = "Disconnect";
            listChats.ItemsSource = Chat("Client is connected!");
        }
    }

    private void btnSend_Click(object sender, EventArgs e)
    {
        listChats.ItemsSource = Chat("Message sent!");
    }
}