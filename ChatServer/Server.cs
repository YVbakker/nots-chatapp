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
    private List<Client> _clients;
    private int _nClients;

    public Server(string hostname, int port, int bufferSize)
    {
        _bufferSize = bufferSize;
        _hostEntry = Dns.GetHostEntry(hostname);
        var address = _hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, port);
        _listenSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clients = new List<Client>();
        _nClients = 0;
    }

    public async Task Start()
    {
        try
        {
            _listenSocket.Bind(_endPoint);
            _listenSocket.Listen(Backlog);
            await Task.Run(async () =>
            {
                while (true)
                {
                    Log.Information("Listening for new connections on {Hostname}:{Port}", _hostEntry.HostName,
                        _endPoint.Port);
                    try
                    {
                        var newClientSocket = await _listenSocket.AcceptAsync();
                        _nClients++;
                        var newClient = new Client {Socket = newClientSocket, Name = $"Client {_nClients}"};
                        _clients.Add(newClient);
                        Task.Run(() => StartHandler(newClient));
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Could not accept new client");
                        throw;
                    }

                    Log.Information("New client added. ({NClients}) currently connected", _clients.Count);
                }
            });
            Log.Information("Server started");
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not start server");
            throw;
        }
    }

    private async Task StartHandler(Client client)
    {
        Log.Information("Started handler thread for client");
        while (true)
        {
            var buffer = new byte[_bufferSize];
            var message = new StringBuilder();
            message.Append($"[{client.Name}] ");
            if (client.Socket.Connected)
            {
                if (client.Socket.Available <= 0) continue;
                do
                {
                    var data = await client.Socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (data is 0) break;
                    var parsed = Encoding.ASCII.GetString(buffer);
                    message.Append(parsed);
                } while (client.Socket.Available > 0);
                NotifyAllClients(message.ToString());
            }
            else
            {
                client.Socket.Close();
                _clients.Remove(client);
                Log.Information("{ClientName} disconnected", client.Name);
                break;
            }
        }
    }

    private async Task NotifyAllClients(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        Log.Information("({Message}) sent to all clients", message);
        foreach (var client in _clients)
        {
            await client.Socket.SendAsync(data, SocketFlags.None);
        }
    }
}