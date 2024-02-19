using BotBase;
using BotBase.Modules;
using Discord.Commands;

namespace DibariBot.Modules.MDSearch;

public class SearchPrefixModule : PrefixModule
{
    [NamedArgumentType]
    public class NameableArguments
    {
        public bool Spoiler { get; set; } = false;

    }

    private readonly SearchService searchService;

    public SearchPrefixModule(SearchService search)
    {
        searchService = search;
    }

    [Command("search")]
    [ParentModulePrefix(typeof(SearchModule))]
    public async Task SearchCommand(string query, NameableArguments args)
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        await ReplyAsync(await searchService.GetMessageContents(new SearchService.State { query = query, isSpoiler = args.Spoiler }));
    }

    [Command("search")]
    [ParentModulePrefix(typeof(SearchModule))]
    public async Task SearchCommand([Remainder] string query)
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        await ReplyAsync(await searchService.GetMessageContents(new SearchService.State { query = query, isSpoiler = false }));
    }
}