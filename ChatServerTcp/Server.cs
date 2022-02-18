using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace ChatServerTcp;

public class Server
{
    private readonly TcpListener _server;
    private readonly List<TcpClient> _clients;
    private int _buffersize;

    public Server(string hostname, int port, int buffersize)
    {
        _buffersize = buffersize;
        var hostEntry = Dns.GetHostEntry(hostname);
        var address = hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        var endPoint = new IPEndPoint(address, port);
        _server = new TcpListener(endPoint);
        _clients = new List<TcpClient>();
    }

    public async Task Start()
    {
        try
        {
            _server.Start();
            await Task.WhenAll(StartListening(), StartReceiving());
            // while (true)
            // {
            //     var client = await _server.AcceptTcpClientAsync();
            //     _clients.Add(client);
            //     _clients.ForEach(async c =>
            //     {
            //         if (c.Available > 0)
            //         {
            //             await ReadMessageAsync(c.GetStream(), c.Available);
            //         }
            //     });
            // }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task StartListening()
    {
        while (true)
        {
            var client = await _server.AcceptTcpClientAsync();
            _clients.Add(client);
            Log.Information("New client connected: {@Endpoint}", client.Client.RemoteEndPoint);
        }
    }

    public async Task StartReceiving()
    {
        while (true)
        {
            foreach (var client in _clients.Where(client => client.Connected && client.Available > 0))
            {
                await ReadMessageAsync(client.GetStream(), client.Available);
            }
        }
    }

    public async Task ReadMessageAsync(NetworkStream stream, int nBytesAvailable)
    {
        Byte[] bytes = new Byte[nBytesAvailable];
        var nBytesRead = await stream.ReadAsync(bytes);
        Log.Information("Received {Bytes} bytes: {Message}", nBytesRead, Encoding.ASCII.GetString(bytes));
        // String data = null;
        // int i;
        // var sb = new StringBuilder();
        // while((i = stream.Read(bytes, 0, bytes.Length))!=0)
        // {
        //     sb.Append(Encoding.ASCII.GetString(bytes, 0, i));
        // }
        // Log.Information(sb.ToString());
    }
}