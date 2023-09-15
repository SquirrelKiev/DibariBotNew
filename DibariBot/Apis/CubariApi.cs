using Newtonsoft.Json;

namespace DibariBot;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class CubariApi
{
    private readonly Uri baseUri;

    private readonly IHttpClientFactory httpFactory;
    private readonly ICacheProvider cache;
    //private readonly BotConfig botConfig;

    public CubariApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        httpFactory = http;
        this.cache = cache;
        //this.botConfig = botConfig;
        baseUri = new(botConfig.CubariUrl);
    }

    /// <summary>
    /// Gets and deserializes the result from whatever url is.
    /// </summary>
    /// <typeparam name="T">the type to deserialize as</typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<T?> Get<T>(string url)
    {
        return await cache.GetOrCreateAsync($"cubari:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url))}", async () =>
        {
            using var client = httpFactory.CreateClient();

            var fullUri = new Uri(baseUri, url);
            Log.Debug("Fetching {url}", fullUri);

            var res = await client.GetAsync(fullUri);

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException($"{((int)res.StatusCode)}: {res.StatusCode}");
            }

            var json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<T>(json);

            return obj;
        }, new CacheValueSettings());
    }

    public string GetUrl(SeriesIdentifier identifier, Bookmark bookmark)
    {
        identifier.ThrowIfInvalid();

        var fullUri = new Uri(baseUri,
            $"/read/{Uri.EscapeDataString(identifier.platform!)}/{Uri.EscapeDataString(identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}");

        return fullUri.ToString();
    }
}
