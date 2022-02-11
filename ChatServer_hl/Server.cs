using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ChatServer_hl;

public class Server
{
    private TcpListener _listener;
    private ConcurrentQueue<TcpClient> _clients;

    public Server(IPEndPoint endpoint)
    {
        _listener = new TcpListener(endpoint);
    }

    public async Task Start()
    {
        _listener.Start();

        while (true)
        {
            if (_listener.Pending())
            {
                var test = await _listener.AcceptTcpClientAsync();
            }
        }
    }
}