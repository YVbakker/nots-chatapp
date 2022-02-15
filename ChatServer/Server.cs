using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace ChatServer;

public class Server
{
    private const int Backlog = 100;
    private int _bufferSize;
    private readonly IPHostEntry _hostEntry;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _listenSocket;
    private List<Socket> _clients;

    public Server(string hostname, int port, int bufferSize)
    {
        _bufferSize = bufferSize;
        _hostEntry = Dns.GetHostEntry(hostname);
        var address = _hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, port);
        _listenSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clients = new List<Socket>();
    }

    public async Task Start()
    {
        try
        {
            await Task.WhenAll(StartListeningAsync(), StartHandlingClientsAsync());
            // await Task.Run(StartHandlingClientsAsync);
            // Log.Information("Server started");
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
                Log.Information("Listening for new connections on {Hostname}:{Port}", _hostEntry.HostName,
                    _endPoint.Port);
                try
                {
                    var newClient = await _listenSocket.AcceptAsync();
                    _clients.Add(newClient);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Could not accept new client");
                    throw;
                }

                Log.Information("New client added. ({NClients}) currently connected", _clients.Count);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not start listener thread");
            throw;
        }
    }

    private async Task StartHandlingClientsAsync()
    {
        while (true)
        {
            await Task.Delay(1); // adding a delay here results in a working loop? Weird behaviour
            // Log.Information("Client handling thread is alive!");
            foreach (var client in _clients)
            {
                if (client.Connected)
                {
                    if (client.Available > 0)
                    {
                        Log.Information("{NBytes} bytes received from client, handling now", client.Available);
                        await HandleMessageAsync(client, client.Available);
                    }
                }
                else
                {
                    Log.Information("Cleaning up disconnected client");
                    try
                    {
                        client.Close();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Could not close the client socket");
                        throw;
                    }

                    _clients.Remove(client);
                    Log.Information("Disconnected client successfully removed and resources freed");
                }
            }
        }
    }

    private async Task HandleMessageAsync(Socket client, int nBytesReceived)
    {
        var buffer = new byte[nBytesReceived];
        // await Task.Factory.FromAsync(client.BeginReceive(buffer, 0, nBytesReceived, SocketFlags.None, null, null), client.EndReceive);
        await client.ReceiveAsync(buffer, SocketFlags.None);
        Log.Information("Received message from client: {Message}", Encoding.ASCII.GetString(buffer));
        // await client.SendAsync(buffer, SocketFlags.None);
        // Log.Information("Sent the message to the client");
        await SendMessageToAllClientsAsync(buffer);
    }

    private async Task SendMessageToAllClientsAsync(byte[] buffer)
    {
        foreach (var client in _clients)
        {
            await client.SendAsync(buffer, SocketFlags.None);
        }
    }
}