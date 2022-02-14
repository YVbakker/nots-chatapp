using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace ChatServer;

public class ServerController
{
    private IList<Client> _clients;
    private ILogger _logger;
    private IPHostEntry? _ipHostInfo;
    private IPAddress? _ipAddress;
    private IPEndPoint? _localEndPoint;

    private Socket? _listener;

    public ServerController()
    {
        _clients = new List<Client>();
        _logger = Log.ForContext<ServerController>();
    }

    public async Task Start()
    {
        try
        {
            _ipHostInfo = await Dns.GetHostEntryAsync("localhost");
            _ipAddress = _ipHostInfo.AddressList.First();
            _localEndPoint = new IPEndPoint(_ipAddress, 11000);

            _listener = new Socket(_ipAddress.AddressFamily,
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
            _listener?.Bind(_localEndPoint ?? throw new InvalidOperationException());
            _listener?.Listen(100);

            while (true)
            {
                Console.WriteLine("Waiting for a connection on " + _ipHostInfo?.HostName);
                await AcceptAsync(_listener ?? throw new InvalidOperationException());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task AcceptAsync(Socket socket)
    {
        Socket? clientSocket = null;
        try
        {
            // clientSocket = await Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, socket);
            clientSocket = await socket.AcceptAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _clients.Add(new Client(clientSocket));
        }

        Console.WriteLine($"Client {_clients.Count} ({clientSocket.LocalEndPoint} -> {clientSocket.RemoteEndPoint}) connected!");
        var task = Task.Run(() => clientSocket.Send(Encoding.ASCII.GetBytes("Client connected!")));
        var nBytesSent = await task;
        Console.WriteLine($"Sent {nBytesSent} bytes to client");
    }

    private async Task StartReceivingAsync()
    {
        while (true)
        {
            foreach (var client in _clients)
            {
                try
                {
                    if (!client.Socket.Connected || client.Socket.Available <= 0) continue;
                    //check nog of client niet al aan het lezen is, anders krijgen we meerdere threads
                    Console.WriteLine($"Received {client.Socket.Available} bytes from {client.Socket.RemoteEndPoint}");
                    var bytes = new byte[client.Socket.Available];
                    var task = Task.Run(() => client.Socket.Receive(bytes));
                    var nBytesReceived = await task;
                    Console.WriteLine($"Received {nBytesReceived} bytes: {Encoding.ASCII.GetString(bytes)}");
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

    public Task<byte[]> ReceiveAsync(Socket sock, int nBytes)
    {
        var buffer = new byte[nBytes];
        sock.Receive(buffer);
        return Task.FromResult(buffer);
    }
}