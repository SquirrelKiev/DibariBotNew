using DibariBot.Apis;

namespace DibariBot.Modules.Manga;

[Inject(ServiceLifetime.Transient)]
public class PhixivManga(PhixivApi api) : IManga
{
    private SeriesIdentifier identifier;

    private PhixivInfoSchema artworkInfo = null!;

    public async Task<IManga> Initialize(SeriesIdentifier id)
    {
        id.ThrowIfInvalid();

        identifier = id;

        var artworkData = await api.GetById(identifier.series!);

        artworkInfo = artworkData ?? throw new NullReferenceException(nameof(artworkData));

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
            artist = artworkInfo.author.name,
            author = artworkInfo.author.name,
            contentRating = MangaAttributesSchema.ContentRating.Unknown,
            description = artworkInfo.description,
            tags = artworkInfo.tags,
            title = artworkInfo.title,
        });
    }

    public string GetUrl(Bookmark bookmark)
    {
        return $"https://www.pixiv.net/en/artworks/{identifier.series}";
    }

    public Task<ChapterSrcs> GetImageSrcs(string chapter)
    {
        return Task.FromResult(new ChapterSrcs(artworkInfo.urls, "Pixiv"));
    }

    public Task<ChapterMetadata> GetChapterMetadata(string chapter)
    {
        return Task.FromResult(new ChapterMetadata
        {
            id = chapter,
            title = artworkInfo.title,
            volume = "N/A"
        });
    }

    public Task<string?> GetNextChapterKey(string currentChapterKey)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<string?> GetPreviousChapterKey(string currentChapterKey)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<string?> DefaultChapter()
    {
        return Task.FromResult<string?>("1");
    }

    public Task<bool> HasChapter(string chapter)
    {
        return Task.FromResult(chapter == "1");
    }
}