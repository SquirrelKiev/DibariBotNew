using BotBase;
using BotBase.Database;
using BotBase.Modules.Help;
using DibariBot.Database;
using Discord.Commands;

namespace DibariBot.Modules.Help;

public class HelpPrefixModule(DbService dbService, LazyHelpService helpService, BotConfigBase botConfig)
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

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : botConfig.DefaultPrefix;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public async Task HelpCommand([Remainder] string sink)
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : botConfig.DefaultPrefix;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }
}