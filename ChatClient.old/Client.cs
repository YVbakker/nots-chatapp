using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient;

public class Client
{
    private int _port;
    private String _hostname;
    private Socket _socket;
    private ChatDelegate _chat;

    public Client(int port, string hostname, ChatDelegate chat)
    {
        _port = port;
        _hostname = hostname;
        _chat = chat;
    }

    public async Task Start()
    {
        try
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(_hostname);
            IPAddress ipAddress = ipHostInfo.AddressList.First();
            IPEndPoint remoteEp = new IPEndPoint(ipAddress, _port);
            _socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(remoteEp);
            await Task.WhenAll(Task.Run(StartReceivingAsync), Task.Run(StartSendingAsync));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public bool Connected()
    {
        return _socket.Connected;
    }

    public async Task SendAsync(string message)
    {
        try
        {
            await _socket.SendAsync(Encoding.ASCII.GetBytes(message), SocketFlags.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task StartReceivingAsync()
    {
        while (true)
        {
            try
            {
                if (_socket.Connected)
                {
                    if (_socket.Available > 0)
                    {
                        var bytes = new byte[_socket.Available];
                        await _socket.ReceiveAsync(bytes, SocketFlags.None);
                        _chat("server", Encoding.ASCII.GetString(bytes));
                    }
                    await _socket.SendAsync(Encoding.ASCII.GetBytes("This is a message"), SocketFlags.None);
                }
                else
                {
                    await _socket.DisconnectAsync(true);
                }
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private async Task StartSendingAsync()
    {
        while (true)
        {
            // _socket.Send(Encoding.ASCII.GetBytes("This is a message"));
            await SendAsync("This is a message");
            await Task.Delay(3000);
        }
    }
}