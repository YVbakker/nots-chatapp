using System;

namespace ChatServer
{
    internal class Program
    {
        private static ServerController _serverController = new ServerController();
        static void Main(string[] args)
        {
            _serverController.Start().Wait();
        }
    }
}