using DibariBot.Database;
using DibariBot.Database.Extensions;
using Discord.Commands;

namespace DibariBot.Modules.Help;

public class HelpPrefixModule : DibariPrefixModule
{
    private readonly DbService dbService;
    private readonly HelpService helpService;

    public HelpPrefixModule(DbService dbService, HelpService helpService)
    {
        this.dbService = dbService;
        this.helpService = helpService;
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public async Task HelpCommand()
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id) : null;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }
}