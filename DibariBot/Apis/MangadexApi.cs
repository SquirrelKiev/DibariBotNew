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

    public async Task GetMangas(MangaListQueryParams queryParams)
    {
        
    }
}
