using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace MyMauiApp;

public static class Client
{
    public static int BufferSize { get; set; }
    public static string Hostname { get; set; } = "localhost";
    public static int Port { get; set; } = 9000;
    private static IPHostEntry _hostEntry;
    private static IPEndPoint _endPoint;
    private static Socket _socket;
    public static WriteLineDelegate WriteLineDelegate { get; set; }

    public static async Task Connect()
    {
        _hostEntry = Dns.GetHostEntry(Hostname);
        var address = _hostEntry.AddressList.FirstOrDefault() ?? IPAddress.Loopback;
        _endPoint = new IPEndPoint(address, Port);
        _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            await _socket.ConnectAsync(_endPoint);
            WriteLineDelegate($"Client connected to {_hostEntry.HostName}:{_endPoint.Port}");
            Log.Information("Client connected to {Hostname}:{Port}", _hostEntry.HostName, _endPoint.Port);
            Task.Run(StartReceiving);
        }
        catch (Exception e)
        {
            Log.Error(e, "Could not connect to server");
            throw;
        }
    }

    public static async Task StartSending()
    {
        WriteLineDelegate("Started sender thread");
        while (true)
        {
            await Task.Delay(1000);
            if (_socket.Connected)
            {
                // Log.Information("Sending message");
                try
                {
                    await _socket.SendAsync(Encoding.ASCII.GetBytes("Hello world!"), SocketFlags.None);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Could not send message");
                    throw;
                }
            }
        }
    }

    public static async Task StartReceiving()
    {
        WriteLineDelegate("Started receiver thread");
        while (true)
        {
            await Task.Delay(1000);
            // if (_socket.Available <= 0) continue;
            WriteLineDelegate("Socket available");
            // var buffer = new byte[_socket.Available];
            // await _socket.ReceiveAsync(buffer, SocketFlags.None);
            // WriteLineDelegate(Encoding.ASCII.GetString(buffer));
            // if (_socket.Connected)
            // {
            //     // WriteLineDelegate("Socket connected");
            //     if (_socket.Available <= 0) continue;
            //     WriteLineDelegate("Socket available");
            //     var buffer = new byte[_socket.Available];
            //     await _socket.ReceiveAsync(buffer, SocketFlags.None);
            //     WriteLineDelegate(Encoding.ASCII.GetString(buffer));
            // }
            // else
            // {
            //     Log.Information("Socket no longer active, closing...");
            //     try
            //     {
            //         _socket.Close();
            //         break;
            //     }
            //     catch (Exception e)
            //     {
            //         Log.Error(e, "Could not close socket");
            //         throw;
            //     }
            // }
        }
    }

    private static async Task HandleMessageAsync(int nBytesReceived)
    {
        var buffer = new byte[nBytesReceived];
        await _socket.ReceiveAsync(buffer, SocketFlags.None);
        WriteLineDelegate(Encoding.ASCII.GetString(buffer));
    }
}