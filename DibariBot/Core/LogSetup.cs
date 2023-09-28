using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

namespace DibariBot;

public static class LogSetup
{
    public static void SetupLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(
#if DEBUG
            Serilog.Events.LogEventLevel.Verbose,
#else
            Serilog.Events.LogEventLevel.Information,
#endif
            theme: AnsiConsoleTheme.Literate)
            .CreateLogger();

        Console.OutputEncoding = Encoding.UTF8;

#if DEBUG
        Log.Debug("This is a development build! Verbose logs are enabled.");
#endif
    }
}
