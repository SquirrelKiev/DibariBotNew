namespace DibariBot;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class MangadexApi
{
    private readonly Api api;

    public MangadexApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        api = new Api(http, cache, botConfig.CubariUrl);
    }

    public Task<T?> Get<T>(string url)
    {
        return api.Get<T>(url);
    }
}
