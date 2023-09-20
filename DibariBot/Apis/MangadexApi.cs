using DibariBot.Apis;

namespace DibariBot;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class MangaDexApi
{
    private readonly Api api;
    private readonly Uri baseUri;

    public MangaDexApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        api = new Api(http, cache);

        baseUri = new Uri(botConfig.MangaDexUrl);
    }

    public async Task<MangaListSchema> GetMangas(MangaListQueryParams queryParams)
    {
        var uri = new Uri(baseUri, "manga?" + QueryStringSerializer.ToQueryParams(queryParams));

        queryParams.includes = new ReferenceExpansionMangaSchema[]
        {
            ReferenceExpansionMangaSchema.Author
        };

        var res = await api.Get<MangaListSchema>(uri);

        return res ?? throw new NullReferenceException();
    }
}
