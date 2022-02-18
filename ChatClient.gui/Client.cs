using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient;

public class Client
{
    private int _bufferSize;
    private readonly IPHostEntry _hostEntry;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _socket;
    private ChatDelegate _writeToChatBox;

    public Client(string hostname, int port, int bufferSize, ChatDelegate del)
    {
        _writeToChatBox = del;
        _bufferSize = bufferSize;
        _hostEntry = Dns.GetHostEntry(hostname);
        var address = _hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, port);
        _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task Connect()
    {
        try
        {
            await _socket.ConnectAsync(_endPoint);
            _writeToChatBox("client", $"Client connected to {_hostEntry.HostName}:{_endPoint.Port}");
            await StartReceiving();
        }
        catch (Exception e)
        {
            _writeToChatBox("error", e.Message);
            throw;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_socket.Connected)
        {
            // Log.Information("Sending message");
            try
            {
                await _socket.SendAsync(Encoding.ASCII.GetBytes(message), SocketFlags.None);
            }
            catch (Exception e)
            {
                _writeToChatBox("error", e.Message);
                throw;
            }
        }
    }

    private async Task StartReceiving()
    {
        _writeToChatBox("debug","Started receiving");
        while (true)
        {
            await Task.Delay(2000).ContinueWith(_ => { _writeToChatBox("thread", "Receiver thread alive");});
            if (_socket.Connected)
            {
                if (_socket.Available > 0)
                {
                    await HandleMessageAsync(_socket.Available);
                }
            }
            else
            {
                try
                {
                    _socket.Close();
                    break;
                }
                catch (Exception e)
                {
                    _writeToChatBox("error", e.Message);
                    throw;
                }
            }
        }
    }

    private async Task HandleMessageAsync(int nBytesReceived)
    {
        var buffer = new byte[nBytesReceived];
        // await Task.Factory.FromAsync(_socket.BeginReceive(buffer, 0, nBytesReceived, SocketFlags.None, null, null), _socket.EndReceive);
        await _socket.ReceiveAsync(buffer, SocketFlags.None);
        _writeToChatBox("server", Encoding.ASCII.GetString(buffer));
        // Console.WriteLine(Encoding.ASCII.GetString(buffer));
    }
}