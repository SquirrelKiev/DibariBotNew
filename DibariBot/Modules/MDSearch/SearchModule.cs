using DibariBot.Modules.Manga;

namespace DibariBot.Modules.MDSearch;

public class SearchModule : DibariModule
{
    private readonly SearchService searchService;
    private readonly MangaService mangaService;

    public SearchModule(SearchService search, MangaService mangaService)
    {
        searchService = search;
        this.mangaService = mangaService;
    }

    [SlashCommand("manga-search", "Searches MangaDex for the query provided. (searches titles, sorted by relevance)")]
    [EnabledInDm(false)]
    public async Task SearchSlash(string query)
    {
        await DeferAsync();

        await FollowupAsync(await searchService.GetMessageContents(new SearchService.State() { query = query }));
    }

    [ComponentInteraction(ModulePrefixes.MANGADEX_SEARCH_BUTTON_PREFIX + "*")]
    public async Task SearchButtonInteraction(string id)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<SearchService.State>(id);

        await ModifyOriginalResponseAsync(await searchService.GetMessageContents(state));
    }

    [ComponentInteraction(ModulePrefixes.MANGADEX_SEARCH_DROPDOWN_PREFIX)]
    public async Task SearchDropdownInteraction(string dexId)
    {
        await DeferAsync();

        await ModifyOriginalResponseAsync(await mangaService.MangaCommand(Context.Guild.Id, Context.Channel.Id, 
            new SeriesIdentifier("mangadex", dexId).ToString()));
    }
}
