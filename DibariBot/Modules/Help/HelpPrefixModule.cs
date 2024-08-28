using DibariBot.Database;
using Discord.Commands;

namespace DibariBot.Modules.Help;

public class HelpPrefixModule(DbService dbService, LazyHelpService helpService)
    : PrefixModule
{
    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public async Task HelpCommand()
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        await using var dbContext = dbService.GetDbContext();


        var config = Context.Guild != null ? await dbContext.GetGuildConfig(Context.Guild.Id) : null;

        var prefix = Context.Guild != null ? config!.Prefix : GuildConfig.DefaultPrefix;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public Task HelpCommand([Remainder] string sink)
    {
        return HelpCommand();
    }
}