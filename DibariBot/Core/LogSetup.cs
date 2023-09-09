using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

namespace DibariBot;

public static class LogSetup
{
    public static void SetupLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information,
            theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        Console.OutputEncoding = Encoding.UTF8;
    }
}
