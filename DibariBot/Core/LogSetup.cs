using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

namespace DibariBot;

public static class LogSetup
{
    public static void SetupLogger()
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Debug()
#endif
            .Enrich.FromLogContext()
            .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose,
            theme: AnsiConsoleTheme.Literate)
            .CreateLogger();

        Console.OutputEncoding = Encoding.UTF8;

#if DEBUG
        Log.Debug("This is a development build! Verbose logs are enabled.");
#endif
    }
}
