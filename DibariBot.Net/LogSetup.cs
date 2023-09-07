using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
