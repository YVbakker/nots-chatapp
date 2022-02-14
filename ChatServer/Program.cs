// See https://aka.ms/new-console-template for more information

using ChatServer;
using Serilog;

using var log = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured");
Log.Information("Attempting to start the server...");
var server = new Server("localhost", 9000, 1024);
await server.Start();

