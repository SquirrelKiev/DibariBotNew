﻿using DibariBot.Database;
using DibariBot.Database.Extensions;

namespace DibariBot.Modules.Help;

public class HelpModule : DibariModule
{
    private readonly DbService dbService;
    private readonly HelpService helpService;

    public HelpModule(DbService dbService, HelpService helpService)
    {
        this.dbService = dbService;
        this.helpService = helpService;
    }

    [SlashCommand("manga-help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    [EnabledInDm(true)]
    public async Task HelpSlash()
    {
        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id) : null;
        var contents = helpService.GetMessageContents(prefix);

        await FollowupAsync(contents);
    }
}
