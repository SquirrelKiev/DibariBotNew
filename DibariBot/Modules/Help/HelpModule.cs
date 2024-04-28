using BotBase;
using BotBase.Database;
using BotBase.Modules.Help;
using DibariBot.Database;

namespace DibariBot.Modules.Help;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class HelpModule(DbService dbService, HelpService helpService, BotConfigBase botConfig)
    : BotModule
{
    [SlashCommand("help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    public async Task HelpSlash()
    {
        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : null;
        var contents = helpService.GetMessageContents(prefix);

        await FollowupAsync(contents);
    }
}
