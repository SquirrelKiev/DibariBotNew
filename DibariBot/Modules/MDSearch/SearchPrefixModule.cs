using Discord.Commands;

namespace DibariBot.Modules.MDSearch;

public class SearchPrefixModule : DibariPrefixModule
{
    [NamedArgumentType]
    public class NameableArguments
    {
        public bool Spoiler { get; set; }= false;
        
    }

    private readonly SearchService searchService;

    public SearchPrefixModule(SearchService search)
    {
        searchService = search;
    }

        [Command("search")]
    public async Task SearchCommand(string query, NameableArguments args)
    {
        await DeferAsync();

        await ReplyAsync(await searchService.GetMessageContents(new SearchService.State { query = query, isSpoiler = args.Spoiler }));
    }

    [Command("search")]
    public async Task SearchCommand([Remainder] string query)
    {
        await DeferAsync();

        await ReplyAsync(await searchService.GetMessageContents(new SearchService.State { query = query, isSpoiler = false }));
    }
}