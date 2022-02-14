using System.Net;
using System.Net.Sockets;
using Serilog;

namespace ChatServer;

public class Server
{
    private const int Backlog = 100;
    private int _bufferSize;
    private readonly IPHostEntry _hostEntry;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _listenSocket;
    private IList<Socket> _clients;

    public Server(string hostname, int port, int bufferSize)
    {
        _bufferSize = bufferSize;
        _hostEntry = Dns.GetHostEntry(hostname);
        var address = _hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, port);
        _listenSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clients = new List<Socket>();
    }

    public void Start()
    {
        try
        {
            StartListeningAsync().Start();
            Log.Information("Server started");
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not start server");
            throw;
        }
    }

    private async Task StartListeningAsync()
    {
        try
        {
            _listenSocket.Bind(_endPoint);
            _listenSocket.Listen(Backlog);
            while (true)
            {
                Log.Information("Listening for new connections on {Hostname}:{Port}", _hostEntry.HostName, _endPoint.Port);
                var newClient = await _listenSocket.AcceptAsync();
                _clients.Add(newClient);
                Log.Information("New client added ({@RemoteAddress})", newClient.RemoteEndPoint);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not start listener thread");
            throw;
        }
    }
}