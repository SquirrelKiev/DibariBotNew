using Discord.Commands;

namespace DibariBot.Modules.About;

public class AboutPrefixModule : DibariPrefixModule
{
    private readonly AboutService aboutService;

    public AboutPrefixModule(AboutService aboutService)
    {
        this.aboutService = aboutService;
    }

    [Command("about")]
    [ParentModulePrefix(typeof(AboutModule))]
    public async Task AboutSlash()
    {
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id);

        await ReplyAsync(contents);
    }
}