using BotBase;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace DibariBot;

public static class Startup
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Log.Logger = new LoggerConfiguration().WriteTo.Console(outputTemplate: "[FALLBACK] [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();

        if (!new BotConfigFactory<BotConfig>().GetConfig(out var botConfig))
        {
            Environment.Exit(1);
        }
        if (!botConfig.IsValid())
        {
            Environment.Exit(1);
        }

        var builder = new HostBuilder();

        var logLevel = botConfig.LogEventLevel;
    }
}