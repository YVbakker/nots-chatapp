using System.Net.Sockets;

namespace ChatServer;

public struct Client
{
    public string Name { get; set; }
    public Socket Socket { get; set; }
}