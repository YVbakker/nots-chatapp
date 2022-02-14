using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class Server
{
    private const int Backlog = 100;
    private int _bufferSize;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _listenSocket;
    private IList<Socket> _clients;

    public Server(IPAddress address, int port, int bufferSize)
    {
        _bufferSize = bufferSize;
        _endPoint = new IPEndPoint(address, port);
        _listenSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clients = new List<Socket>();
    }

    public void Start()
    {
        StartListeningAsync().Start();
        
    }

    private async Task StartListeningAsync()
    {
        try
        {
            _listenSocket.Bind(_endPoint);
            _listenSocket.Listen(Backlog);
            while (true)
            {
                var newClient = await _listenSocket.AcceptAsync();
                _clients.Add(newClient);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}