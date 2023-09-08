using Newtonsoft.Json.Linq;

namespace DibariBot.Mangas;

public class CubariManga : IManga
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Author { get; private set; }
    public string Artist { get; private set; }
    public string Cover { get; private set; }
    public Dictionary<string, string> Groups { get; private set; }
    public SeriesIdentifier Identifier { get => identifier; set => identifier = value; }
    private SeriesIdentifier identifier;
    /// <summary>
    /// use <seealso cref="CubariApi.GetImageSrcs(Manga, string)"/> if you want to get the values from this.
    /// </summary>
    public readonly SortedDictionary<string, CubariChapterSchema> chapters;
    private readonly CubariApi cubari;

    public CubariManga(CubariMangaSchema cubariMangaSchema, string platform, CubariApi cubari)
    {
        Title = cubariMangaSchema.title;
        Description = cubariMangaSchema.description;
        Author = cubariMangaSchema.author;
        Artist = cubariMangaSchema.artist;
        Cover = cubariMangaSchema.cover;
        Groups = cubariMangaSchema.groups;

        chapters = new SortedDictionary<string, CubariChapterSchema>(cubariMangaSchema.chapters, new ChapterNameComparer());

        Identifier = new SeriesIdentifier(platform, cubariMangaSchema.slug);
        this.cubari = cubari;
    }

    public async Task<ChapterSrcs> GetImageSrcs(string chapter)
    {
        var chapterObj = chapters[chapter];

        var group = chapterObj.groups.First();

        var srcs = await GetImageSrcsFromGroup(group.Value);

        return new(srcs, Groups[group.Key]);
    }

    /// <exception cref="NotImplementedException">If the type we get from the api is something we don't handle</exception>
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

    public Task<string[]> GetChapterNames()
    {
        return Task.FromResult(chapters.Keys.ToArray());
    }

    public ChapterMetadata GetChapterMetadata(string chapter)
    {
        var chapterData = chapters[chapter];

        return new ChapterMetadata()
        {
            title = chapterData.title,
            volume = chapterData.volume
        };
    }

    public MangaMetadata GetMetadata()
    {
        return new MangaMetadata()
        {
            author = Author,
            title = Title
        };
    }

    public string GetUrl(Bookmark bookmark)
    {
        return cubari.GetUrl(identifier, bookmark);
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