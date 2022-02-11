using System.Net;
using System.Net.Sockets;
using Serilog;

namespace ChatClient;

public class Client
{
    private readonly ILogger _logger;
    private int _port;
    private String _hostname;
    private Socket _socket;

    public Client(int port, string hostname)
    {
        _logger = Log.ForContext<Client>();
        _port = port;
        _hostname = hostname;
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

            await ConnectAsync(_socket, remoteEp);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task ConnectAsync(Socket socket, IPEndPoint endPoint)
    {
        await Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, endPoint, null);
        _logger.Information("Client Connected!");
    }

    public bool Connected()
    {
        return _socket.Connected;
    }
}