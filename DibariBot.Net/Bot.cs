using DibariBot.Commands.Manga;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace DibariBot;

public class Bot
{
    public DiscordSocketClient Client { get; }
    public InteractionService Commands { get; }

    private readonly IServiceProvider services;

    public Bot()
    {
        Client = new(new()
        {
            GatewayIntents = GatewayIntents.Guilds
        });
        Commands = new(Client, new());

        services = CreateServices();
    }

    private IServiceProvider CreateServices()
    {
        static void DefaultHttpClientConfig(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Dibari/DiscordBot (https://github.com/SquirrelKiev/DibariBot)");
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        var collection = new ServiceCollection()
            .AddSingleton(Client)
            .AddSingleton(Commands)
            .AddSingleton<CubariApi>()
            .AddSingleton<MangaHandler>()
            ;

        collection.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .ConfigureHttpClient(DefaultHttpClientConfig);

        collection.AddHttpClient(CubariApi.CUBARI_CLIENT_NAME)
            .ConfigureHttpClient((client) =>
            {
                DefaultHttpClientConfig(client);
                client.BaseAddress = new Uri("https://cubari.moe");
            });

        return collection.BuildServiceProvider();
    }

    public async Task RunAndBlockAsync()
    {
        await RunAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private async Task RunAsync()
    {
        Client.Log += Client_Log;

        Client.Ready += Client_Ready;
        Client.InteractionCreated += Client_InteractionCreated;
        Commands.InteractionExecuted += Commands_InteractionExecuted;

        await Client.LoginAsync(Discord.TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
        await Client.StartAsync();
    }

    private Task Client_Log(LogMessage message)
    {
        LogEventLevel level;

        switch (message.Severity)
        {
            case LogSeverity.Critical:
                level = LogEventLevel.Fatal;
                break;
            case LogSeverity.Error:
                level = LogEventLevel.Error;
                break;
            case LogSeverity.Warning:
                level = LogEventLevel.Warning;
                break;
            case LogSeverity.Info:
                level = LogEventLevel.Information;
                break;
            case LogSeverity.Verbose:
                level = LogEventLevel.Verbose;
                break;
            case LogSeverity.Debug:
                level = LogEventLevel.Debug;
                break;
            default:
                level = LogEventLevel.Information;
                break;
        }

        if (message.Exception is not null)
        {
            Log.Write(level, message.Exception, "{Source} | {Message}", message.Source, message.Message);
        }
        else
        {
            Log.Write(level, "{Source} | {Message}", message.Source, message.Message);
        }
        return Task.CompletedTask;
    }

    private async Task Client_InteractionCreated(SocketInteraction arg)
    {
        var ctx = new SocketInteractionContext(Client, arg);

        await Commands.ExecuteCommandAsync(ctx, services);
    }

    private async Task Client_Ready()
    {
        await InitializeInteractionService();
    }

    private Task Commands_InteractionExecuted(ICommandInfo cmdInfo, Discord.IInteractionContext ctx, IResult res)
    {
        if (res.IsSuccess)
        {
            Log.Information($"Command {cmdInfo.Module.Name}.{cmdInfo.MethodName} successfully executed.");
        }
        else
        {
            Log.Information($"Command {cmdInfo?.Module?.Name}.{cmdInfo?.MethodName} failed. {res.Error}, {res.ErrorReason}.");
            ctx.Interaction.ModifyOriginalResponseAsync((x) => { x.Content = $"{res.Error}, {res.ErrorReason}"; });
        }

        return Task.CompletedTask;
    }

    private async Task InitializeInteractionService()
    {
        await Commands.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);
        await Commands.RegisterCommandsGloballyAsync(true);
    }
}

