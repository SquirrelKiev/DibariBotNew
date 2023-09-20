namespace DibariBot.Modules.MDSearch;

public class SearchModule : DibariModule
{
    private readonly MangaDexApi mangaDexApi;

    public SearchModule(MangaDexApi mdapi)
    {
        mangaDexApi = mdapi;
    }

    [SlashCommand("manga-search", "Searches MangaDex for the query provided. (searches titles, sorted by relevance)")]
    public async Task SearchSlash(string query)
    {
        await DeferAsync();

        var res = await mangaDexApi.GetMangas(new Apis.MangaListQueryParams
        {
            limit = 10,
            offset = 0,
            order = new Apis.MangaListQueryOrder()
            {
                relevance = Apis.MangaListQueryOrder.QueryOrderSchema.Descending
            },
            title = query
        });

        await FollowupAsync(res.data[0].attributes.title.First().Value);
    }
}
