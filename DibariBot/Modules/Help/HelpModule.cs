using DibariBot.Database;
using Discord.Interactions;

namespace DibariBot.Modules.Help;

[CommandContextType(
    InteractionContextType.Guild,
    InteractionContextType.BotDm,
    InteractionContextType.PrivateChannel
)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class HelpModule(DbService dbService, LazyHelpService helpService) : BotModule
{
    [SlashCommand("help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    public async Task HelpSlash()
    {
        await DeferAsync();

        string prefix = GuildConfig.DefaultPrefix;
        GuildConfig? config = null;
        if (Context.Guild != null)
        {
            await using var dbContext = dbService.GetDbContext();
            config = await dbContext.GetGuildConfig(Context.Guild.Id);

            prefix = config.Prefix;
        }

        MessageContents contents = helpService.GetMessageContents(prefix, config);

        await FollowupAsync(contents);
    }
}
