// See https://aka.ms/new-console-template for more information

using ChatServerTcp;
using Serilog;

using var log = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured");

var server = new Server("localhost", 9000, 2);
await server.Start();