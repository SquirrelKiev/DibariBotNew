using Newtonsoft.Json;

namespace DibariBot.Modules.Manga;

// not a manga but "book" or "comic" sounds less
public class XkcdManga : IManga
{
    private const string XKCD_URL = "https://xkcd.com";
    private readonly Uri xkcdUri = new(XKCD_URL);

    private SeriesIdentifier identifier;

    private readonly IHttpClientFactory httpFactory;

    public XkcdManga(IHttpClientFactory factory)
        => httpFactory = factory;

    public Task<IManga> Initialize(SeriesIdentifier identifier)
    {
        this.identifier = identifier;

        return Task.FromResult((IManga)this);
    }

    public SeriesIdentifier GetIdentifier()
    {
        return identifier;
    }

    public Task<MangaMetadata> GetMetadata()
    {
        return Task.FromResult(new MangaMetadata()
        {
            title = "xkcd",
            author = "Randall Munroe"
        });
    }

    public string GetUrl(Bookmark bookmark)
    {
        return new Uri(xkcdUri, $"/{bookmark.chapter}").ToString();
    }

    public async Task<ChapterSrcs> GetImageSrcs(string chapter)
    {
        var comic = await GetComic(chapter);

        if (!comic.HasValue)
        {
            throw new NullReferenceException("Comic doesnt exist!");
        }

        return new ChapterSrcs(new string[] { comic.Value.imageSrc }, "xkcd");
    }

    public async Task<ChapterMetadata> GetChapterMetadata(string chapter)
    {
        var comic = await GetComic(chapter);

        if (!comic.HasValue)
        {
            throw new NullReferenceException("Comic doesnt exist!");
        }

        return new ChapterMetadata()
        {
            title = comic.Value.title,
            volume = "1",
            id = comic.Value.num.ToString()
        };
    }

    public async Task<string?> GetNextChapterKey(string currentChapterKey)
    {
        if (!int.TryParse(currentChapterKey, out int asInt))
        {
            return null;
        }

        asInt++;

        var comic = await GetComic(asInt.ToString());

        if (comic == null)
        {
            return null;
        }

        return asInt.ToString();
    }

    public async Task<string?> GetPreviousChapterKey(string currentChapterKey)
    {
        if (!int.TryParse(currentChapterKey, out int asInt))
        {
            return null;
        }

        asInt--;

        var comic = await GetComic(asInt.ToString());

        if (comic == null)
        {
            return null;
        }

        return asInt.ToString();
    }

    public Task<string> DefaultChapter()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasChapter(string chapter)
    {
        var comic = await GetComic(chapter);

        if (comic == null)
        {
            return false;
        }

        return true;
    }

    private async Task<XkcdComic?> GetComic(string num)
    {
        switch (num.ToLowerInvariant())
        {
            case "latest":
                return await GetLatestComic();
            case "random":
                return await GetComic((await GetRandomComicId()).ToString());
        };

        var client = httpFactory.CreateClient();

        var req = await client.GetAsync(new Uri(xkcdUri, $"/{num}/info.0.json"));

        if (!req.IsSuccessStatusCode)
        {
            return null;
        }

        var comicInfo = JsonConvert.DeserializeObject<XkcdComic>(await req.Content.ReadAsStringAsync());

        return comicInfo;
    }

    private async Task<XkcdComic> GetLatestComic()
    {
        var client = httpFactory.CreateClient();

        var req = await client.GetAsync(new Uri(xkcdUri, $"/info.0.json"));

        var comicInfo = JsonConvert.DeserializeObject<XkcdComic>(await req.Content.ReadAsStringAsync());

        return comicInfo;
    }

    private async Task<int> GetRandomComicId()
    {
        var latest = await GetLatestComic();

        return Random.Shared.Next(1, latest.num);
    }

    public struct XkcdComic
    {
        public int num;
        public string day;
        public string month;
        public string year;

        [JsonProperty("safe_title")]
        public string title;

        [JsonProperty("img")]
        public string imageSrc;

        public string alt;
    }
}
