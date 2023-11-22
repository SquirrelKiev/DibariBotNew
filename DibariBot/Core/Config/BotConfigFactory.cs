using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DibariBot;

public class BotConfigFactory
{
    private readonly string configPath = Environment.GetEnvironmentVariable("DIBARI_CONFIG_LOCATION") ??
                                         Path.Combine(Path.Combine(AppContext.BaseDirectory, "data"), "bot_config.yaml");

    public bool GetConfig([NotNullWhen(true)] out BotConfig? botConfig)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted)
            .Build();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        if (!File.Exists(configPath))
        {
            botConfig = new BotConfig();

            Log.Fatal("Config not found. Creating new config at {ConfigPath}. Please edit this file and restart the bot.", configPath);

            File.WriteAllText(configPath, serializer.Serialize(botConfig));
            return false;
        }

        try
        {
            botConfig = deserializer.Deserialize<BotConfig>(File.ReadAllText(configPath));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to read config.");
            botConfig = new BotConfig();
            return false;
        }

        //#if DEBUG
        //botConfig.GenerateMetadata();
        File.WriteAllText(configPath, serializer.Serialize(botConfig));
        //#endif

        return true;

    }
}
