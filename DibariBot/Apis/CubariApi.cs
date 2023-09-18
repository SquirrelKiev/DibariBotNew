namespace DibariBot;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class CubariApi
{
    private readonly Api api;
    //private readonly BotConfig botConfig;

    public CubariApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        //this.botConfig = botConfig;
        api = new Api(http, cache, botConfig.CubariUrl);
    }

    
    public Task<T?> Get<T>(string url)
    {
        return api.Get<T>(url);
    }

    public string GetUrl(SeriesIdentifier identifier, Bookmark bookmark)
    {
        identifier.ThrowIfInvalid();

        var fullUri = new Uri(api.baseUri,
            $"/read/{Uri.EscapeDataString(identifier.platform!)}/{Uri.EscapeDataString(identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}");

        return fullUri.ToString();
    }
}
