namespace DibariBot;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class CubariApi
{
    private readonly Api api;
    private readonly Uri baseUri;

    //private readonly BotConfig botConfig;

    public CubariApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        //this.botConfig = botConfig;
        api = new Api(http, cache);

        baseUri = new Uri(botConfig.CubariUrl);
    }

    
    public Task<T?> Get<T>(string url, CacheValueSettings? cvs = null)
    {
        return api.Get<T>(new Uri(baseUri, url), cvs);
    }

    public string GetUrl(SeriesIdentifier identifier, Bookmark bookmark)
    {
        identifier.ThrowIfInvalid();

        var fullUri = new Uri(baseUri,
            $"/read/{Uri.EscapeDataString(identifier.platform!)}/{Uri.EscapeDataString(identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}");

        return fullUri.ToString();
    }
}
