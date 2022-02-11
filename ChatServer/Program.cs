using System;

namespace ChatServer
{
    internal class Program
    {
        private static Server _server = new Server();
        static void Main(string[] args)
        {
            _server.Start().Wait();
        }
    }
}