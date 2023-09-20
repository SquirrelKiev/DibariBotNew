namespace DibariBot.Modules.MDSearch;

public class SearchModule : DibariModule
{
    private readonly SearchService searchService;

    public SearchModule(SearchService search)
    {
        searchService = search;
    }

    [SlashCommand("manga-search", "Searches MangaDex for the query provided. (searches titles, sorted by relevance)")]
    public async Task SearchSlash(string query)
    {
        await DeferAsync();

        await FollowupAsync(await searchService.GetMessageContents(new SearchService.State() { query = query }));
    }
}
