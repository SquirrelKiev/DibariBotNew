using DibariBot.Database;
using DibariBot.Modules.ConfigCommand.Pages;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

namespace DibariBot;

// A heavy thanks to [Nadeko](https://gitlab.com/Kwoth/nadekobot) for being a valuable reference
public class Bot
{
    public DiscordSocketClient Client { get; }
    public InteractionService InteractionService { get; }
    public CommandService CommandService { get; }
    public BotConfig Config { get; }

    private readonly IServiceProvider services;

    public Bot(BotConfig config)
    {
        Config = config;

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | 
                             GatewayIntents.MessageContent | 
                             GatewayIntents.GuildMessages | 
                             GatewayIntents.DirectMessages,
            LogLevel = LogSeverity.Verbose
        });
        InteractionService = new InteractionService(Client, new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Verbose,
            DefaultRunMode = Discord.Interactions.RunMode.Async
        });
        CommandService = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Verbose,
            DefaultRunMode = Discord.Commands.RunMode.Async
        });

        services = CreateServices();
        Log.Information("Services created.");
    }

    private IServiceProvider CreateServices()
    {
        var collection = new ServiceCollection()
            .AddCache(Config)
            .AddSingleton(Config)
            .AddSingleton<DbService>()
            .AddSingleton(Client)
            .AddSingleton(InteractionService)
            .AddSingleton(CommandService)
            ;

        collection.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .ConfigureHttpClient(DefaultHttpClientConfig);

        // Stupid but im not sure how to do this with one scan and pass the inject option's param to WithLifetime
        collection.Scan(scan => scan.FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Singleton)
            )
            .AsSelf()
            .WithSingletonLifetime()
        );

        collection.Scan(scan => scan.FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Transient)
            )
            .AsSelf()
            .WithTransientLifetime()
        );

        collection.Scan(scan => scan.FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.AssignableTo<ConfigPage>())
            .As<ConfigPage>()
            .WithTransientLifetime());

        return collection.BuildServiceProvider();

        static void DefaultHttpClientConfig(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Dibari/DiscordBot (https://github.com/SquirrelKiev/DibariBotNew)");
            client.Timeout = TimeSpan.FromSeconds(10);
        }
    }

    public async Task RunAndBlockAsync()
    {
        Log.Information("Starting bot...");
        await RunAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private async Task RunAsync()
    {
        await services.GetRequiredService<DbService>().Initialize();

#if DEBUG
        if (Environment.GetCommandLineArgs().Contains("nukedb"))
        {
            Log.Debug("Nuking the DB...");

            await services.GetRequiredService<DbService>().ResetDatabase();

            Log.Debug("Nuked!");
        }
#endif

        Client.Log += Client_Log;

        Client.Ready += Client_Ready;

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

    private async Task Client_Ready()
    {
        Log.Information("Logged in as {user}#{discriminator} ({id})", Client.CurrentUser?.Username, Client.CurrentUser?.Discriminator, Client.CurrentUser?.Id);

        await services.GetRequiredService<CommandHandler>().OnReady();
    }
}