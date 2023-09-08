using DibariBot.Mangas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DibariBot;

public class CubariApi
{
    public const string CUBARI_CLIENT_NAME = "Cubari";

    private readonly IHttpClientFactory httpFactory;

    public CubariApi(IHttpClientFactory http)
    {
        httpFactory = http;
    }

    public virtual async Task<CubariManga> GetManga(SeriesIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
        ArgumentNullException.ThrowIfNull(identifier.platform, nameof(identifier.platform));
        ArgumentNullException.ThrowIfNull(identifier.series, nameof(identifier.series));

        string url = $"read/api/{Uri.EscapeDataString(identifier.platform)}/series/{Uri.EscapeDataString(identifier.series)}";

        var mangaRes = await Get<CubariMangaSchema>(url);

        return new CubariManga(mangaRes, identifier.platform, this);
    }

    /// <summary>
    /// Gets and deserializes the result from whatever url is.
    /// </summary>
    /// <typeparam name="T">the type to deserialize as</typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public virtual async Task<T?> Get<T>(string url)
    {
        using var client = httpFactory.CreateClient(CUBARI_CLIENT_NAME);

        var res = await client.GetAsync(url);

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException($"{res.StatusCode}");
        }

        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<T>(json);

        return obj;
    }

    public virtual string GetUrl(SeriesIdentifier identifier, Bookmark bookmark)
    {
        identifier.ThrowIfInvalid();

        // TODO: Not this
        using var client = httpFactory.CreateClient(CUBARI_CLIENT_NAME);

        return $"{client.BaseAddress}read/" +
            $"{Uri.EscapeDataString(identifier.platform!)}/{Uri.EscapeDataString(identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}";
    }
}
