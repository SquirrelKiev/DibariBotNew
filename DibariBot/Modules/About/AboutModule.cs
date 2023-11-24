namespace DibariBot.Modules.About;

public class AboutModule : DibariModule
{
    private readonly AboutService aboutService;

    public AboutModule(AboutService aboutService)
    {
        this.aboutService = aboutService;
    }

    [SlashCommand("about", "Info about the bot.")]
    [HelpPageDescription("Pulls up info about the bot.")]
    [EnabledInDm(true)]
    public async Task AboutSlash()
    {
        await DeferAsync();

        var contents = aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client));

        await FollowupAsync(contents);
    }
}