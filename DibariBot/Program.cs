using DibariBot;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

if (!new BotConfigFactory().GetConfig(out var botConfig))
{
    Environment.Exit(1);
}
if (!botConfig.IsValid())
{
    Environment.Exit(1);
}
var config = botConfig;

DibariBot.LogSetup.SetupLogger(config);

await new Bot(config).RunAndBlockAsync();