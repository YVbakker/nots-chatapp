// See https://aka.ms/new-console-template for more information

using ChatClient;
using Serilog;

using var log = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured");
Log.Information("Attempting to connect client to server");
var client = new Client("localhost", 9000, 1024);

await client.Connect();
await Task.WhenAll(client.StartSending(), client.StartReceiving());