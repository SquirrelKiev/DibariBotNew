using Discord.Interactions;

namespace DibariBot.Modules.About;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class AboutModule(AboutService aboutService) : BotModule
{
    [SlashCommand("about", "Info about the bot.")]
    [HelpPageDescription("Pulls up info about the bot.")]
    public async Task AboutSlash()
    {
        await DeferAsync();

        var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id, Context.Guild);

        await FollowupAsync(contents);
    }
}