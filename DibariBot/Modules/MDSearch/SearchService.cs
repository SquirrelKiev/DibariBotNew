namespace DibariBot.Modules.MDSearch;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class SearchService
{
    public struct State
    {
        public string query;
        public int page;
    }

    private readonly MangaDexApi mangaDexApi;
    private readonly BotConfig config;

    public SearchService(MangaDexApi api, BotConfig config)
    {
        mangaDexApi = api;
        this.config = config;
    }

    public async Task<MessageContents> GetMessageContents(State state)
    {
        var res = await mangaDexApi.GetMangas(new Apis.MangaListQueryParams
        {
            limit = config.MangaDexSearchLimit,
            offset = state.page * config.MangaDexSearchLimit,
            order = new Apis.MangaListQueryOrder()
            {
                relevance = Apis.MangaListQueryOrder.QueryOrderSchema.Descending
            },
            title = state.query
        });

        var embed = new EmbedBuilder();

        foreach(var manga in res.data)
        {
            embed.AddField(manga.attributes.title.ToString()
                .StringOrDefault("No title (why?)")
                .Truncate(config.MaxTitleLength), 
                manga.attributes.description.ToString()
                .StringOrDefault("No description.")
                .Truncate(config.MaxDescriptionLength));
        }

        var components = new ComponentBuilder()
            .WithButton(new ButtonBuilder()
                .WithLabel("<")
                .WithCustomId(ModulePrefixes.MANGADEX_SEARCH_PREFIX +
                StateSerializer.SerializeObject(new State()
                {
                    page = state.page - 1,
                    query = state.query
                }))
                .WithStyle(ButtonStyle.Primary))
            .WithButton(new ButtonBuilder()
                .WithLabel(">")
                .WithCustomId(ModulePrefixes.MANGADEX_SEARCH_PREFIX + 
                StateSerializer.SerializeObject(new State()
                {
                    page = state.page + 1,
                    query = state.query
                }))
                .WithStyle(ButtonStyle.Primary))
            .WithRedButton();

        return new MessageContents(string.Empty, embed: embed.Build(), components);
    }
}
