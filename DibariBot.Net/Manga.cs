using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace DibariBot;

public class Manga
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Author { get; private set; }
    public string Artist { get; private set; }
    public string Cover { get; private set; }
    public Dictionary<string, string> Groups { get; private set; }
    public SeriesIdentifier Identifier { get; private set; }
    /// <summary>
    /// use <seealso cref="CubariApi.GetImageSrcs(Manga, string)"/> if you want to get the values from this.
    /// </summary>
    public readonly SortedDictionary<string, CubariChapterSchema> chapters;

    public Manga(CubariMangaSchema cubariMangaSchema, string platform)
    {
        Title = cubariMangaSchema.title;
        Description = cubariMangaSchema.description;
        Author = cubariMangaSchema.author;
        Artist = cubariMangaSchema.artist;
        Cover = cubariMangaSchema.cover;
        Groups = cubariMangaSchema.groups;

        chapters = new SortedDictionary<string, CubariChapterSchema>(cubariMangaSchema.chapters, new ChapterNameComparer());

        Identifier = new SeriesIdentifier(platform, cubariMangaSchema.slug);
    }
}