using DibariBot.Database;

namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandModule : DibariModule
{
    private readonly DbService dbService;

    public ConfigCommandModule(DbService dbService)
    {
        this.dbService = dbService;
    }

    [SlashCommand("manga-config", "Test command")]
    public async Task ConfigSlash()
    {
        await DeferAsync();

        using var context = dbService.GetDbContext();

        var farts = context.GuildConfig.Add(new Database.Models.GuildConfig() { GuildId = 1074000869881823304ul });

        await context.SaveChangesAsync();

        await FollowupAsync(farts.Entity.GuildId.ToString());
    }
}
