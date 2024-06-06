using BotBase;
using Microsoft.Extensions.Logging;

namespace DibariBot.Apis;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class CubariApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig, ILogger<CubariApi> logger)
{
    private readonly Api api = new(http, cache, logger);
    private readonly Uri baseUri = new(botConfig.CubariUrl);

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
