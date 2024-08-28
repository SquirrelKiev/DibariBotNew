using DibariBot.Database;
using Discord.Interactions;

namespace DibariBot.Modules.Help;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class HelpModule(DbService dbService, LazyHelpService helpService)
    : BotModule
{
    [SlashCommand("help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    public async Task HelpSlash()
    {
        await DeferAsync();

        await using var dbContext = dbService.GetDbContext();

        var config = await dbContext.GetGuildConfig(Context.Guild.Id);

        var prefix = Context.Guild != null ? config.Prefix : GuildConfig.DefaultPrefix;
        var contents = helpService.GetMessageContents(prefix);

        await FollowupAsync(contents);
    }
}
