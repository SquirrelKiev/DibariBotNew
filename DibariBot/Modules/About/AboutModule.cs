using BotBase;
using BotBase.Modules.About;

namespace DibariBot.Modules.About;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class AboutModule(AboutService aboutService, OverrideTrackerService overrideTrackerService)
: AboutModuleImpl(aboutService, overrideTrackerService)
{
    [SlashCommand("about", "Info about the bot.")]
    [HelpPageDescription("Pulls up info about the bot.")]
    public override Task AboutSlash()
    {
        return base.AboutSlash();
    }
}