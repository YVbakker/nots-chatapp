using System;

namespace ConsoleClient // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        public static Client Client;
        static void Main(string[] args)
        {
            Client = new Client(11000, "localhost", Chat);
            Client.Start().Wait();
        }
        
        public static void Chat(string sender, string message)
        {
            Console.WriteLine($"[{sender}] {message}\n");
        }
    }
}