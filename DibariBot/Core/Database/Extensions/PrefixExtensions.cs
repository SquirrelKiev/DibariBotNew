using DibariBot.Database.Models;

namespace DibariBot.Database.Extensions;

public static class PrefixExtensions
{
    public static async Task SetPrefix(this DbService dbService, ulong guildId, string prefix)
    {
        await using var context = dbService.GetDbContext();

        await context.GuildConfig.UpsertAsync(x => x.GuildId == guildId, config => config.Prefix = prefix);

        await context.SaveChangesAsync();
    }

    public static async Task<string> GetPrefix(this DbService dbService, ulong guildId)
    {
        await using var context = dbService.GetDbContext();

        var guild = await context.GuildConfig.GetOrAddAsync(x => x.GuildId == guildId, () => new GuildConfig
        {
            GuildId = guildId,
            Prefix = CommandHandler.DEFAULT_PREFIX
        });
        
        await context.SaveChangesAsync();

        return guild.Prefix;
    }
}
