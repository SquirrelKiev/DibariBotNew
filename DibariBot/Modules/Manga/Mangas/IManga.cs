namespace DibariBot.Modules.Manga;

// Should this be Task or ValueTask?
public interface IManga
{
    public Task<IManga> Initialize(SeriesIdentifier identifier);

    public SeriesIdentifier GetIdentifier();
    public Task<MangaMetadata> GetMetadata();
    public string GetUrl(Bookmark bookmark);

    public Task<ChapterSrcs> GetImageSrcs(string chapter);
    public Task<ChapterMetadata> GetChapterMetadata(string chapter);

    public Task<string?> GetNextChapterKey(string currentChapterKey);
    public Task<string?> GetPreviousChapterKey(string currentChapterKey);

    public Task<string> DefaultChapter();

    public Task<bool> HasChapter(string chapter);
}

public struct ChapterMetadata
{
    public string id;
    public string title;
    public string volume;
}

public struct MangaMetadata
{
    public string title;
    public string author;
    public string description;
    public string artist;
}

public class Cover
{
    public string url = "";
    public string volume = "";
    public string description = "";
}