using DibariBot;

// temp logger in case BotConfig fails
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

if (!new BotConfigFactory().GetConfig(out var botConfig))
{
    Environment.Exit(1);
}
if (!botConfig.IsValid())
{
    Environment.Exit(1);
}

LogSetup.SetupLogger(botConfig);

await new Bot(botConfig).RunAndBlockAsync();