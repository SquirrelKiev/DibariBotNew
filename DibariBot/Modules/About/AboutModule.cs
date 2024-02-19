using BotBase;
using BotBase.Modules.About;

namespace DibariBot.Modules.About;

public class AboutModule : AboutModuleImpl
{
    public AboutModule(AboutService aboutService, OverrideTrackerService overrideTrackerService) : base(aboutService, overrideTrackerService)
    {
    }

    [SlashCommand("about", "Info about the bot.")]
    [HelpPageDescription("Pulls up info about the bot.")]
    [EnabledInDm(true)]
    public override Task AboutSlash()
    {
        return base.AboutSlash();
    }
}