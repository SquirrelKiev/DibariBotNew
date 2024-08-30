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

        string prefix = GuildConfig.DefaultPrefix;
        GuildConfig? config = null;
        if (Context.Guild != null)
        {
            await using var dbContext = dbService.GetDbContext();
            config = await dbContext.GetGuildConfig(Context.Guild.Id);

            prefix = config.Prefix;
        }

        MessageContents contents = helpService.GetMessageContents(prefix, config);

        await ReplyAsync(contents);
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public Task HelpCommand([Remainder] string sink)
    {
        return HelpCommand();
    }
}