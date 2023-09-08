namespace DibariBot.Mangas;

public interface IManga
{
    public SeriesIdentifier Identifier { get; protected set; }

    Task<ChapterSrcs> GetImageSrcs(string chapter);
    Task<string[]> GetChapterNames();
    ChapterMetadata GetChapterMetadata(string chapter);
    MangaMetadata GetMetadata();
    string GetUrl(Bookmark bookmark);
}

public struct ChapterMetadata
{
    internal string title;
    internal string volume;
}

public struct MangaMetadata
{
    internal string title;
    internal string author;
}