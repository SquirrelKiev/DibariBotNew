using Discord.Commands;

namespace DibariBot.Modules.About;

public class AboutPrefixModule(AboutService aboutService) : PrefixModule
{
    [Command("about")]
    [ParentModulePrefix(typeof(AboutModule))]
    public async Task AboutCommand()
    {
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id, Context.Guild);

        await ReplyAsync(contents);
    }
}