using Discord.Commands;

namespace DibariBot.Modules.MDSearch;

public class SearchPrefixModule : DibariPrefixModule
{
    private readonly SearchService searchService;

    public SearchPrefixModule(SearchService search)
    {
        searchService = search;
    }

    [Command("search")]
    public async Task SearchCommand([Remainder] string query)
    {
        await DeferAsync();

        await ReplyAsync(await searchService.GetMessageContents(new SearchService.State { query = query }));
    }
}