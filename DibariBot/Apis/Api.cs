using Newtonsoft.Json;

namespace DibariBot;

public class Api
{
    public readonly Uri baseUri;
    public readonly IHttpClientFactory httpFactory;
    public readonly ICacheProvider cache;

    public Api(IHttpClientFactory http, ICacheProvider cache, string baseUrl)
    {
        httpFactory = http;
        this.cache = cache;
        baseUri = new Uri(baseUrl);
    }

    /// <summary>
    /// Gets and deserializes the result from whatever url is.
    /// </summary>
    /// <typeparam name="T">the type to deserialize as</typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public virtual async Task<T?> Get<T>(string url)
    {
        var fullUri = new Uri(baseUri, url);

        return await cache.GetOrCreateAsync($"apiCache:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fullUri.ToString()))}", async () =>
        {
            Log.Debug("Fetching {url}", fullUri);

            using var client = httpFactory.CreateClient();

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
}
