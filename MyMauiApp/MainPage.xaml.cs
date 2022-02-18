using Serilog;

namespace MyMauiApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Client.WriteLineDelegate = WriteLine;
        chatBox.IsReadOnly = true;
        chatBox.Text = "Welcome to ChatClient, press 'connect' to get started.\n";
    }

    public void WriteLine(string message)
    {
        chatBox.Text += $"{message}\n";
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        try
        {
            await Client.Connect();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Could not connect to server");
        }
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        Task.Run(Client.StartSending);
        WriteLine("Sending");
    }
}