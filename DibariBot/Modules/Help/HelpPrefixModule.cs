using BotBase;
using BotBase.Database;
using BotBase.Modules.Help;
using DibariBot.Database;
using Discord.Commands;

namespace DibariBot.Modules.Help;

public class HelpPrefixModule : PrefixModule
{
    private readonly DbService dbService;
    private readonly HelpService helpService;
    private readonly BotConfigBase botConfig;

    public HelpPrefixModule(DbService dbService, HelpService helpService, BotConfigBase botConfig)
    {
        this.dbService = dbService;
        this.helpService = helpService;
        this.botConfig = botConfig;
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public async Task HelpCommand()
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : null;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }
}