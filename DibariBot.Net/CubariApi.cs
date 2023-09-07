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

    public virtual async Task<Manga> GetManga(SeriesIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
        ArgumentNullException.ThrowIfNull(identifier.platform, nameof(identifier.platform));
        ArgumentNullException.ThrowIfNull(identifier.series, nameof(identifier.series));

        string url = $"read/api/{Uri.EscapeDataString(identifier.platform)}/series/{Uri.EscapeDataString(identifier.series)}";

        var mangaRes = await Get<CubariMangaSchema>(url);

        return new Manga(mangaRes, identifier.platform);
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

    public virtual string GetUrl(Manga manga, Bookmark bookmark)
    {
        manga.Identifier.ThrowIfInvalid();

        // TODO: Not this
        using var client = httpFactory.CreateClient(CUBARI_CLIENT_NAME);

        return $"{client.BaseAddress}read/" +
            $"{Uri.EscapeDataString(manga.Identifier.platform!)}/{Uri.EscapeDataString(manga.Identifier.series!)}/{Uri.EscapeDataString(bookmark.chapter!)}/{bookmark.page + 1}";
    }

    // i dont usually comment this much but i keep forgetting how this works and spending 20 minutes figuring it out again
    public async Task<Bookmark> Navigate(Manga manga, Bookmark currentLocation, int deltaChapters, int deltaPages)
    {
        ArgumentNullException.ThrowIfNull(currentLocation, nameof(currentLocation));
        ArgumentNullException.ThrowIfNull(currentLocation.chapter, nameof(currentLocation.chapter));
        ArgumentNullException.ThrowIfNull(currentLocation.page, nameof(currentLocation.page));

        // is sorted
        var chapterKeys = manga.chapters.Keys.ToList();
        var currentChapterIndex = chapterKeys.IndexOf(currentLocation.chapter);

        var newChapterIndex = Math.Clamp(currentChapterIndex + deltaChapters, 0, chapterKeys.Count - 1);
        int newPage = (deltaChapters != 0) ? 0 : currentLocation.page;

        while(deltaPages != 0)
        {
            var pageUrls = await GetImageSrcs(manga, chapterKeys[newChapterIndex]);

            if(deltaPages > 0)
            {
                // going forward

                // can we fulfil the entirety of deltaPages within this chapter
                if (newPage + deltaPages < pageUrls.srcs.Length)
                {
                    newPage += deltaPages;
                    deltaPages = 0;
                }
                else
                {
                    // we cant, subtract all of this chapter's remaining pages from deltaPages and move to the next chapter
                    deltaPages -= (pageUrls.srcs.Length - newPage);
                    newPage = 0;
                    newChapterIndex++;
                }
            }
            else
            {
                // going backwards

                // can we fulfil the entirety of deltaPages within this chapter
                if (newPage + deltaPages >= 0)
                {
                    newPage += deltaPages;
                    deltaPages = 0;
                }
                else
                {
                    // we cant

                    // to beginning of chapter to be safe
                    deltaPages += newPage + 1;
                    newChapterIndex--;
                    if (newChapterIndex >= 0)
                    {
                        pageUrls = await GetImageSrcs(manga, chapterKeys[newChapterIndex]);
                        newPage = pageUrls.srcs.Length - 1;
                    }
                    else
                    {
                        // beginning of manga
                        newPage = 0;
                        deltaPages = 0;
                    }
                }
            }

            if (newChapterIndex < 0 || newChapterIndex >= chapterKeys.Count)
            {
                // if we reach the beginning or end of the manga, stop adjusting
                deltaPages = 0;
            }
        }

        return new Bookmark(chapterKeys[newChapterIndex], newPage);


        //var chapterIndex = chapterKeys.IndexOf(currentLocation.chapter);

        //var newPage = currentLocation.page;

        //var skipToEnd = false;

        //void SetChapterIndex(int newValue)
        //{
        //    if (newValue < 0)
        //    {
        //        chapterIndex = 0;
        //        newPage = 0;
        //    }
        //    else if (newValue > chapterKeys.Count - 1)
        //    {
        //        chapterIndex = chapterKeys.Count - 1;
        //        skipToEnd = true;
        //    }
        //    else
        //    {
        //        chapterIndex = newValue;
        //    }
        //}

        //if (deltaChapters != 0)
        //{
        //    SetChapterIndex(chapterIndex + deltaChapters);
        //    newPage = 0;
        //}

        //while (true)
        //{
        //    var totalChapterPages = (await GetImageSrcs(manga, currentLocation.chapter)).srcs.Length;

        //    if (skipToEnd)
        //    {
        //        newPage = totalChapterPages - 1;
        //        break;
        //    }
        //    else if (newPage <= 0)
        //    {
        //        SetChapterIndex(chapterIndex - 1);
        //        newPage += (await GetImageSrcs(manga, chapterKeys[chapterIndex])).srcs.Length;
        //    }
        //    else if (newPage > totalChapterPages)
        //    {
        //        SetChapterIndex(chapterIndex + 1);
        //        newPage -= totalChapterPages;
        //    }
        //    else
        //    {
        //        break;
        //    }
        //}

        //return new Bookmark(chapterKeys[chapterIndex], newPage);
    }

    public async Task<ChapterSrcs> GetImageSrcs(Manga manga, string chapter)
    {
        var chapterObj = manga.chapters[chapter];

        var group = chapterObj.groups.First();

        var srcs = await GetImageSrcsFromGroup(group.Value);

        return new(srcs, manga.Groups[group.Key]);
    }

    /// <exception cref="NotImplementedException">If the type we get from the api is something we don't handle</exception>
    private async Task<string[]> GetImageSrcsFromGroup(JToken token)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        // sure hope no one does any weird recursion nonsense :clueless:
        if (token.Type == JTokenType.String)
        {
            string url = token.ToString();

            var req = await Get<JToken>(url);

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
    public Dictionary<string, Newtonsoft.Json.Linq.JToken> groups;

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