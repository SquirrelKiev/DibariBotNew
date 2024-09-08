using DibariBot.Database;

namespace DibariBot.Modules;

[Inject(ServiceLifetime.Singleton)]
public class ColorProvider(BotConfig config, DbService dbService)
{
    public async Task<Color> GetEmbedColor(IGuild? guild)
    {
        await using var dbContext = dbService.GetDbContext();

        return await GetEmbedColor(dbContext, guild);
    }

    public async Task<Color> GetEmbedColor(BotDbContext dbContext, IGuild? guild)
    {
        var guildConfig = guild == null ? null : await dbContext.GetGuildConfig(guild.Id);

        return GetEmbedColor(guildConfig);
    }

    public Color GetEmbedColor(GuildConfig? guildConfig)
    {
        return guildConfig?.EmbedColor == null
            ? new Color((uint)config.DefaultEmbedColor)
            : new Color((uint)guildConfig.EmbedColor);
    }

    public Color GetErrorEmbedColor()
    {
        return (uint)config.ErrorEmbedColor;
    }
}
