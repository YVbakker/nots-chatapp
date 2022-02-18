// See https://aka.ms/new-console-template for more information

using ChatClientTcp;
using Serilog;

using var log = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured");

var client = new Client("localhost", 9000, 1024);
await client.Start();