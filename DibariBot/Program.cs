using BotBase;
using DibariBot;
using Serilog.Events;

// temp logger in case BotConfig fails
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

if (!new BotConfigFactory<BotConfig>().GetConfig(out var botConfig))
{
    Environment.Exit(1);
}
if (!botConfig.IsValid())
{
    Environment.Exit(1);
}

var logLevel =
#if DEBUG
    LogEventLevel.Verbose;
#else
    LogEventLevel.Information;
#endif

LogSetup.SetupLogger(botConfig, logLevel);

#if DEBUG
Log.Debug("This is a development build! Verbose logs are enabled.");
#endif

await new Bot(botConfig).RunAndBlockAsync();