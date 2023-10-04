using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

namespace DibariBot;

public static class LogSetup
{
    public static void SetupLogger(BotConfig config)
    {
        var logConfig = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(
#if !DEBUG
            Serilog.Events.LogEventLevel.Information,
#endif
            theme: AnsiConsoleTheme.Literate)
            ;

        if (!string.IsNullOrWhiteSpace(config.SeqUrl))
        {
            logConfig.WriteTo.Seq(config.SeqUrl, apiKey: config.SeqApiKey);
        }

        Log.Logger = logConfig.CreateLogger();

        Console.OutputEncoding = Encoding.UTF8;

#if DEBUG
        Log.Debug("This is a development build! Verbose logs are enabled.");
#endif
    }
}
