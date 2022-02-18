using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace ChatClientTcp;

public class Client
{
    private readonly TcpClient _client;
    private readonly IPEndPoint _endPoint;
    private int _buffersize;

    public Client(string hostname, int port, int buffersize)
    {
        _buffersize = buffersize;
        var hostEntry = Dns.GetHostEntry(hostname);
        var address = hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, port);
        _client = new TcpClient();
    }

    public async Task Start()
    {
        try
        {
            await _client.ConnectAsync(_endPoint);
            await Task.WhenAll(StartSendingAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task StartSendingAsync()
    {
        while (true)
        {
            // await Task.Delay(3000);
            // Log.Information("Sender thread is alive and the client is {Connected}",
                // _client.Connected ? "connected" : "disconnected");
            if (!_client.Connected) continue;
            try
            {
                await _client.GetStream().WriteAsync(Encoding.ASCII.GetBytes("Hello world!"));
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not send message to server");
                throw;
            }
        }
    }
}