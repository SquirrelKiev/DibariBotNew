using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tomlyn;

namespace DibariBot;

public class BotConfigFactory
{
    private readonly string configPath;

    public BotConfigFactory()
    {
        configPath = Environment.GetEnvironmentVariable("DIBARI_CONFIG_LOCATION") ?? 
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "bot_config.toml");
    }

    public bool GetConfig([NotNullWhen(true)] out BotConfig? botConfig)
    {
        if (!File.Exists(configPath))
        {
            botConfig = new BotConfig();
            botConfig.GenerateMetadata();

            Log.Fatal("Config not found. Created new config at {ConfigPath}. Please edit this file and restart the bot.", configPath);

            File.WriteAllText(configPath, Toml.FromModel(botConfig));

            return false;
        }
        else if (Toml.TryToModel(File.ReadAllText(configPath), out botConfig, out var diagnostics))
        {
            return true;
        }
        else
        {
            Log.Fatal("Failed to read config. Diagnostics below.");
            foreach(var diag in diagnostics)
            {
                var level = diag.Kind switch
                {
                    Tomlyn.Syntax.DiagnosticMessageKind.Error => LogEventLevel.Error,
                    Tomlyn.Syntax.DiagnosticMessageKind.Warning => LogEventLevel.Warning,
                    _ => LogEventLevel.Error,
                };

                Log.Write(level, "Toml | {DiagSpan}: {DiagMessage}", diag.Span.ToStringSimple(), diag.Message);
            }

            return false;
        }
    }
}
