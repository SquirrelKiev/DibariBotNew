using Newtonsoft.Json.Linq;

namespace DibariBot.Modules.Manga;

[Inject(ServiceLifetime.Transient)]
public class CubariManga : IManga
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Author { get; private set; }
    public string Artist { get; private set; }
    public string Cover { get; private set; }
    public Dictionary<string, string> Groups { get; private set; }

    private SeriesIdentifier identifier;
    private SortedList<string, CubariChapterSchema> chapters;

    private readonly CubariApi cubari;
    private readonly ICacheProvider cache;

#pragma warning disable CS8618 // null error, technically true but only if Initialize() wasnt called which in that case, it should throw anyway
    public CubariManga(CubariApi cubari, ICacheProvider cache)
    {
        this.cubari = cubari;
        this.cache = cache;
    }
#pragma warning restore CS8618

    public async Task<IManga> Initialize(SeriesIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
        ArgumentNullException.ThrowIfNull(identifier.platform, nameof(identifier.platform));
        ArgumentNullException.ThrowIfNull(identifier.series, nameof(identifier.series));

        string url = $"read/api/{Uri.EscapeDataString(identifier.platform)}/series/{Uri.EscapeDataString(identifier.series)}";

        var mangaRes = await cubari.Get<CubariMangaSchema>(url);

        Title = mangaRes.title;
        Description = mangaRes.description;
        Author = mangaRes.author;
        Artist = mangaRes.artist;
        Cover = mangaRes.cover;
        Groups = mangaRes.groups;

        chapters = new SortedList<string, CubariChapterSchema>(mangaRes.chapters, new ChapterNameComparer());

        this.identifier = new SeriesIdentifier(identifier.platform, mangaRes.slug);

        return this;
    }

    public SeriesIdentifier GetIdentifier()
    {
        return identifier;
    }
    public Task<MangaMetadata> GetMetadata()
    {
        return Task.FromResult(new MangaMetadata()
        {
            author = Author,
            title = Title
        });
    }
    public string GetUrl(Bookmark bookmark)
    {
        return cubari.GetUrl(identifier, bookmark);
    }

    /// <exception cref="NotImplementedException">If the type we get from the api is something we don't handle</exception>
    public async Task<ChapterSrcs> GetImageSrcs(string chapter)
    {
        var chapterObj = chapters[chapter];

        var group = chapterObj.groups.First();

        var srcs = await GetImageSrcsFromGroup(group.Value);

        return new(srcs, Groups[group.Key]);
    }
    private async Task<string[]> GetImageSrcsFromGroup(JToken token)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        // sure hope no one does any weird recursion nonsense :clueless:
        if (token.Type == JTokenType.String)
        {
            string url = token.ToString();

            var req = await cubari.Get<JToken>(url);

            return await GetImageSrcsFromGroup(req!);
        }
        else if (token.Type == JTokenType.Array)
        {
            if (token.All(t => t.Type == JTokenType.String))
            {
                string[] imageUrls = token.ToObject<string[]>()!;

                return imageUrls;
            }
            else if (token.All(t => t.Type == JTokenType.Object))
            {
                CubariImgurSchema[] imgurObjects = token.ToObject<CubariImgurSchema[]>()!;

                return imgurObjects.Select((x) => x.src).ToArray();
            }
        }

        // no clue what on earth we've recieved, bail
        throw new NotImplementedException();
    }
    public Task<ChapterMetadata> GetChapterMetadata(string chapter)
    {
        var chapterData = chapters[chapter];

        return Task.FromResult(new ChapterMetadata()
        {
            id = chapter,
            title = chapterData.title,
            volume = chapterData.volume
        });
    }

    public Task<string?> GetNextChapterKey(string currentChapterKey)
    {
        var nextChapterIndex = chapters.IndexOfKey(currentChapterKey) + 1;

        if (nextChapterIndex >= chapters.Count)
        {
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(chapters.GetKeyAtIndex(nextChapterIndex));
    }
    public Task<string?> GetPreviousChapterKey(string currentChapterKey)
    {
        var previousChapterKey = chapters.IndexOfKey(currentChapterKey) - 1;

        if (previousChapterKey < 0)
        {
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(chapters.GetKeyAtIndex(previousChapterKey));
    }

    public Task<string> DefaultChapter()
    {
        return Task.FromResult(chapters.Keys.First());
    }

    public Task<bool> HasChapter(string chapter)
    {
        return Task.FromResult(chapters.ContainsKey(chapter));
    }

}

public struct CubariMangaSchema
{
    public string slug, title, description, author, artist, cover, series_name;
    public Dictionary<string, string> groups;
    public Dictionary<string, CubariChapterSchema> chapters;
}

public struct CubariChapterSchema
{
    public string volume;
    public string title;
    public Dictionary<string, JToken> groups;

    public Dictionary<string, long> release_date;

    // [JsonConverter(typeof(UnixDateTimeConverter))]
    public long last_updated;
}

public struct CubariImgurSchema
{
    public string description, src;
}

public struct ChapterSrcs
{
    public string[] srcs;
    public string group;

    public ChapterSrcs(string[] srcs, string group)
    {
        ArgumentNullException.ThrowIfNull(srcs, nameof(srcs));

        this.srcs = srcs;
        this.group = group;
    }
}