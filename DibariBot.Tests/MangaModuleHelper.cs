namespace DibariBot.Tests;

internal static class MangaModuleHelper
{
    public static CubariMangaSchema GetValidMangaSchema()
    {
        var schema = new CubariMangaSchema()
        {
            artist = "TestArtist",
            author = "TestAuthor",
            series_name = "TestManga",
            title = "TestManga",
            slug = ValidSeriesIdentifier.series!,
            cover = "https://example.com/testcover.png",
            description = "This is a really cool manga",
            groups = new()
            {
                { "1", "TestGroup1" },
                { "2", "TestGroup2" }
            },
            chapters = new()
            {
                { "1", new  }
            }
        };

        return schema;
    }

    public static CubariChapterSchema GetValidChapterSchema()
    {
        var schema = new CubariChapterSchema()
        {
            groups = new()
            {
                { "1", "/read/api/" }
            }
        };

        return schema;
    }

    public static SeriesIdentifier ValidSeriesIdentifier
    {
        get
        {
            var schema = new SeriesIdentifier("testplatform", "testseries");

            return schema;
        }
    }
}
