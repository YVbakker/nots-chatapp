// See https://aka.ms/new-console-template for more information

using ChatServer;
using Serilog;

namespace ChatServer // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = log;

            var hostname = "localhost";
            var port = 9000;
            var bufferSize = 1024;

            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("Usage: chatServer <hostname> <port> <buffersize>");
                    return 1;
                case 1:
                    try
                    {
                        hostname = args[0];
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Please enter a valid string for hostname");
                        throw;
                    }

                    break;
                case 2:
                    try
                    {
                        hostname = args[0];
                        port = int.Parse(args[1]);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Please enter a valid string for hostname, number for port");
                        throw;
                    }

                    break;
                case 3:
                    try
                    {
                        hostname = args[0];
                        port = int.Parse(args[1]);
                        bufferSize = int.Parse(args[2]);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e,
                            "Please enter a valid string for hostname and a number for portnumber, buffersize");
                        throw;
                    }

                    hostname = args[0];
                    break;
            }

            Log.Information("The global logger has been configured");
            Log.Information("Attempting to start the server...");
            Server server;
            try
            {
                server = new Server(hostname, port, bufferSize);
                await server.Start();
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start server");
                throw;
            }
            return 0;
        }
    }
}

