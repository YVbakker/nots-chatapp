using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace ChatClient;

public class Client
{
    private int _bufferSize;
    private readonly IPHostEntry _hostEntry;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _socket;

    public Client(string hostname, int port, int bufferSize)
    {
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
            Log.Information("Client connected to {Hostname}:{Port}", _hostEntry.HostName, _endPoint.Port);
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not connect to server");
            throw;
        }
    }

    public async Task StartSending()
    {
        while (true)
        {
            await Task.Delay(1000);
            if (_socket.Connected)
            {
                // Log.Information("Sending message");
                try
                {
                    await _socket.SendAsync(Encoding.ASCII.GetBytes("Hello world!"), SocketFlags.None);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Could not send message");
                    throw;
                }
            }
        }
    }

    public async Task StartReceiving()
    {
        while (true)
        {
            if (_socket.Connected)
            {
                if (_socket.Available > 0)
                {
                    await HandleMessageAsync(_socket.Available);
                }
            }
            else
            {
                Log.Information("Socket no longer active, closing...");
                try
                {
                    _socket.Close();
                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Could not close socket");
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
        Console.WriteLine(Encoding.ASCII.GetString(buffer));
    }
}