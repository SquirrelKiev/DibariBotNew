namespace DibariBot.Modules.About;

public class AboutModule : DibariModule
{
    private readonly AboutService aboutService;

    public AboutModule(AboutService aboutService)
    {
        this.aboutService = aboutService;
    }

    [ComponentInteraction(ModulePrefixes.ABOUT_OVERRIDE_TOGGLE)]
    [EnabledInDm(true)]
    public async Task OverrideToggleButton()
    {
        await DeferAsync();

        if (await aboutService.TryToggleOverride(Context.User.Id))
        {
            var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id);

            await ModifyOriginalResponseAsync(contents);
        }
        else
        {
            await RespondAsync(new MessageContents("No permission.", embed: null, null), true);
        }
    }

    [SlashCommand("about", "Info about the bot.")]
    [HelpPageDescription("Pulls up info about the bot.")]
    [EnabledInDm(true)]
    public async Task AboutSlash()
    {
        await DeferAsync();

        var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id);

        await FollowupAsync(contents);
    }
}