using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Serilog;

namespace ChatServer;

public class Server
{
    private ConcurrentBag<Client> _clients;
    private ILogger _logger;
    private IPHostEntry? _ipHostInfo;
    private IPAddress? _ipAddress;
    private IPEndPoint? _localEndPoint;

    private Socket? listener;

    public Server()
    {
        _clients = new ConcurrentBag<Client>();
        _logger = Log.ForContext<Server>();
    }

    public async Task Start()
    {
        try
        {
            _ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
            _ipAddress = _ipHostInfo.AddressList.First();
            _localEndPoint = new IPEndPoint(_ipAddress, 11000);

            listener = new Socket(_ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var listenerThread = Task.Run(StartListeningAsync);
        var receiverThread = Task.Run(StartReceivingAsync);
        await Task.WhenAll(listenerThread, receiverThread);
    }

    private async Task StartListeningAsync()
    {
        try
        {
            listener?.Bind(_localEndPoint ?? throw new InvalidOperationException());
            listener?.Listen(100);

            while (true)
            {
                Console.WriteLine("Waiting for a connection on " + _ipHostInfo?.HostName);
                await AcceptAsync(listener ?? throw new InvalidOperationException());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task AcceptAsync(Socket socket)
    {
        var clientSocket = await socket.AcceptAsync();
        _clients.Add(new Client(clientSocket));
        Console.WriteLine($"Client {_clients.Count} ({socket.LocalEndPoint} -> {socket.RemoteEndPoint}) connected!");
    }

    private async Task StartReceivingAsync()
    {
        while (true)
        {
            foreach (var client in _clients)
            {
                try
                {
                    if (client.Socket.Connected && client.Socket.Available > 0)
                    {
                        Console.WriteLine($"Received {client.Socket.Available} bytes from {client.Socket.RemoteEndPoint}");
                        await client.Socket.ReceiveAsync(client.Buffers, SocketFlags.None);
                    }
                }
                catch (ObjectDisposedException e)
                {
                    //TODO remove disconnected socket from _clients
                    // (ConcurrentBag is probably not the best collection for this, but a dictionary is overkill?)
                    // maybe keep socket in bag but close it / mark as disconnected and skip it
                    // client.Socket.Close();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}