// See https://aka.ms/new-console-template for more information

using Serilog;

Console.WriteLine("Hello, World!");

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured");