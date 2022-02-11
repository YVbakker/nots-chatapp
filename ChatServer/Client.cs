using System.Net.Sockets;

namespace ChatServer;

public class Client
{
    public string Name { get; set; }
    public Socket Socket { get; set; }
    public IList<ArraySegment<byte>> Buffers;

    public Client(Socket socket)
    {
        this.Socket = socket;
        this.Buffers = new List<ArraySegment<byte>>();
    }
}