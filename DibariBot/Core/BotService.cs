using System.Reflection;
using DibariBot.Database;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DibariBot;

public class BotService(
    DiscordSocketClient client,
    BotConfig config,
    DbService dbService,
    ILogger<BotService> logger,
    CommandHandler commandHandler
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var args = Environment.GetCommandLineArgs();
        var migrationEnabled = !(args.Contains("nomigrate") || args.Contains("nukedb"));
        await dbService.Initialize(migrationEnabled);

#if DEBUG
        if (Environment.GetCommandLineArgs().Contains("nukedb"))
        {
            logger.LogDebug("Nuking the DB...");

            await dbService.ResetDatabase();

            logger.LogDebug("Nuked!");
        }
#endif

        client.Log += Client_Log;

        client.Ready += Client_Ready;

        await client.LoginAsync(TokenType.Bot, config.BotToken);
        await client.StartAsync();
    }

    private Task Client_Log(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information,
        };

        if (message.Exception is not null)
        {
            logger.Log(
                level,
                message.Exception,
                "{Source} | {Message}",
                message.Source,
                message.Message
            );
        }
        else
        {
            logger.Log(level, "{Source} | {Message}", message.Source, message.Message);
        }
        return Task.CompletedTask;
    }

    private async Task Client_Ready()
    {
        logger.LogInformation(
            "Logged in as {user}#{discriminator} ({id})",
            client.CurrentUser?.Username,
            client.CurrentUser?.Discriminator,
            client.CurrentUser?.Id
        );

        await commandHandler.OnReady(Assembly.GetExecutingAssembly());
    }
}
