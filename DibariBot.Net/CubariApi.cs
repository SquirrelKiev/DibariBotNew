using DibariBot.Mangas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DibariBot;

public class CubariApi
{
    private const string CUBARI_URL = "https://cubari.moe";
    private readonly Uri baseUri = new(CUBARI_URL);

    private readonly IHttpClientFactory httpFactory;

    public CubariApi(IHttpClientFactory http)
    {
        httpFactory = http;
    }

    /// <summary>
    /// Gets and deserializes the result from whatever url is.
    /// </summary>
    /// <typeparam name="T">the type to deserialize as</typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<T?> Get<T>(string url)
    {
        using var client = httpFactory.CreateClient();

        var fullUri = new Uri(baseUri, url);

        var res = await client.GetAsync(fullUri);

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException($"{res.StatusCode}");
        }

        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<T>(json);

        return obj;
    }

    public string GetUrl(SeriesIdentifier identifier, Bookmark bookmark)
    {
        identifier.ThrowIfInvalid();

        var fullUri = new Uri(baseUri, 
            $"/read/{Uri.EscapeDataString(identifier.platform!)}/{Uri.EscapeDataString(identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}");

        return fullUri.ToString();
    }
}
