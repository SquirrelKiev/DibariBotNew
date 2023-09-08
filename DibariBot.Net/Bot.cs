using DibariBot.Commands.Manga;
using DibariBot.Mangas;
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
    public BotConfig Config { get; }

    private readonly IServiceProvider services;

    public Bot()
    {
        Client = new(new()
        {
            GatewayIntents = GatewayIntents.Guilds
        });
        Commands = new(Client, new());

        if (!new BotConfigFactory().GetConfig(out var botConfig))
        {
            Environment.Exit(1);
        }
        if (!botConfig.IsValid())
        {
            Environment.Exit(1);
        }
        Config = botConfig;
        Log.Information("Bot config loaded.");

        services = CreateServices();
        Log.Information("Services created.");
    }

    private IServiceProvider CreateServices()
    {
        static void DefaultHttpClientConfig(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Dibari/DiscordBot (https://github.com/SquirrelKiev/DibariBot)");
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        var collection = new ServiceCollection()
            .AddSingleton(Config)
            .AddSingleton(Client)
            .AddSingleton(Commands)
            .AddSingleton<CubariApi>()
            .AddSingleton<MangaFactory>()
            .AddSingleton<MangaService>()
            ;

        collection.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .ConfigureHttpClient(DefaultHttpClientConfig);

        collection.Scan(scan => scan.FromAssemblyOf<IManga>()
            .AddClasses(classes => classes.AssignableToAny(
                typeof(IManga)
                )
            )
            .AsSelfWithInterfaces()
            .WithTransientLifetime()
        );

        return collection.BuildServiceProvider();
    }

    public async Task RunAndBlockAsync()
    {
        Log.Information("Starting bot...");
        await RunAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private async Task RunAsync()
    {
        Client.Log += Client_Log;

        Client.Ready += Client_Ready;
        Client.InteractionCreated += Client_InteractionCreated;
        Commands.InteractionExecuted += Commands_InteractionExecuted;

        await Client.LoginAsync(TokenType.Bot, Config.BotToken);
        await Client.StartAsync();
    }

    private Task Client_Log(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information,
        };

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

    private Task Commands_InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, IResult res)
    {
        if (res.IsSuccess)
        {
            Log.Information("Command {ModuleName}.{MethodName} successfully executed.", cmdInfo.Module.Name, cmdInfo.MethodName);
        }
        else
        {
            if(res is ExecuteResult executeResult)
            {
                Log.Error(executeResult.Exception, "Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.", 
                    cmdInfo?.Module?.Name, cmdInfo?.MethodName, executeResult.Error, executeResult.ErrorReason);
            }
            else
            {
                Log.Error("Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                    cmdInfo?.Module?.Name, cmdInfo?.MethodName, res.Error, res.ErrorReason);
            }
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