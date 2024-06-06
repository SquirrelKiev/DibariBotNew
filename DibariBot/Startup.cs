using BotBase;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using BotBase.Database;
using BotBase.Modules.About;
using BotBase.Modules.ConfigCommand;
using BotBase.Modules.Help;
using DibariBot.Database;
using DibariBot.Modules.ConfigCommand.Pages;
using DibariBot.Modules.ConfigCommand;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DibariBot;

public static class Startup
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        Console.OutputEncoding = Encoding.UTF8;
        Log.Logger = new LoggerConfiguration().WriteTo.Console(outputTemplate: "[FALLBACK] [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();

        if (!new BotConfigFactory<BotConfig>().GetConfig(out var botConfig))
        {
            Environment.Exit(1);
        }
        if (!botConfig.IsValid())
        {
            Environment.Exit(1);
        }

        var builder = new HostBuilder();

        var logLevel = botConfig.LogEventLevel;

        var logConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithThreadName()
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
        ;

        if (!string.IsNullOrWhiteSpace(botConfig.SeqUrl))
        {
            logConfig.WriteTo.Seq(botConfig.SeqUrl, apiKey: botConfig.SeqApiKey);
        }

        var logger = logConfig.CreateLogger();
        builder.ConfigureLogging(logging =>
            logging.AddSerilog(logger)
                .AddFilter<SerilogLoggerProvider>("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogLevel.Warning)
                .AddFilter<SerilogLoggerProvider>("Microsoft.EntityFrameworkCore.*", LogLevel.Warning))
            ;

        builder.ConfigureServices(x => x.AddBotServices(botConfig));
        builder.ConfigureHostConfiguration(configBuilder => configBuilder.AddEnvironmentVariables(prefix: "DOTNET_"));

        await builder.RunConsoleAsync();
    }

    private static IServiceCollection AddBotServices(this IServiceCollection serviceCollection, BotConfig botConfig)
    {
        serviceCollection
            .AddSingleton<BotConfigBase>(botConfig)
            .AddCache(botConfig)
            .AddSingleton(botConfig)
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.MessageContent |
                                 GatewayIntents.GuildMessageReactions |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.DirectMessages |
                                 GatewayIntents.GuildMembers,
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig()
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = Discord.Interactions.RunMode.Async
                }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = Discord.Commands.RunMode.Async
            }))
            .AddSingleton<DbService>()
            .AddSingleton(x => (DbServiceBase<BotDbContext>)x.GetService<DbService>()!)
            .AddSingleton<CommandHandler>()
            // for help command
            .AddSingleton<OverrideTrackerService>()
            .AddSingleton<HelpService>()
            // about command
            .AddSingleton<AboutService>()
            // config command
            .AddSingleton<ConfigCommandService>()
            .AddSingleton(x => (ConfigCommandServiceBase<ConfigPage.Page>)x.GetService<ConfigCommandService>()!);
        ;

        serviceCollection.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .ConfigureHttpClient(DefaultHttpClientConfig);

        serviceCollection.ConfigureHttpClientDefaults(x => x.RemoveAllLoggers());

        // Stupid but im not sure how to do this with one scan and pass the inject option's param to WithLifetime
        serviceCollection.Scan(scan => scan.FromAssemblyOf<BotService>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Singleton)
            )
            .AsSelf()
            .WithSingletonLifetime()
        );

        serviceCollection.Scan(scan => scan.FromAssemblyOf<BotService>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Transient)
            )
            .AsSelf()
            .WithTransientLifetime()
        );

        serviceCollection.Scan(scan => scan.FromAssemblyOf<BotService>()
            .AddClasses(classes => classes.AssignableTo<ConfigPage>())
            .As<ConfigPage>()
            .As<ConfigPageBase<ConfigPage.Page>>()
            .WithTransientLifetime());

        serviceCollection.AddHostedService<BotService>();

        return serviceCollection;

        static void DefaultHttpClientConfig(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Dibari/DiscordBot (https://github.com/SquirrelKiev/DibariBotNew)");
            client.Timeout = TimeSpan.FromSeconds(10);
        }
    }

}