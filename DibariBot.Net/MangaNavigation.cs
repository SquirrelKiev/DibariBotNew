using DibariBot.Mangas;
using System.Diagnostics.CodeAnalysis;

namespace DibariBot;

internal static class MangaNavigation
{
    // i dont usually comment this much but i keep forgetting how this works and spending 20 minutes figuring it out again
    public static async Task<Bookmark> Navigate(IManga manga, Bookmark currentLocation, int deltaChapters, int deltaPages)
    {
        ArgumentNullException.ThrowIfNull(currentLocation, nameof(currentLocation));
        ArgumentNullException.ThrowIfNull(currentLocation.chapter, nameof(currentLocation.chapter));
        ArgumentNullException.ThrowIfNull(currentLocation.page, nameof(currentLocation.page));

        // is sorted
        var chapterKeys = await manga.GetChapterNames();
        var currentChapterIndex = Array.IndexOf(chapterKeys, currentLocation.chapter);

        var newChapterIndex = Math.Clamp(currentChapterIndex + deltaChapters, 0, chapterKeys.Length - 1);
        int newPage = (deltaChapters != 0) ? 0 : currentLocation.page;

        while (deltaPages != 0)
        {
            var pageUrls = await manga.GetImageSrcs(chapterKeys[newChapterIndex]);

            if (deltaPages > 0)
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

                    deltaPages += newPage + 1;
                    newChapterIndex--;
                    if (newChapterIndex >= 0)
                    {
                        pageUrls = await manga.GetImageSrcs(chapterKeys[newChapterIndex]);
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

            if (newChapterIndex < 0 || newChapterIndex >= chapterKeys.Length)
            {
                // if we reach the beginning or end of the manga, stop adjusting
                deltaPages = 0;
            }
        }

        return new Bookmark(chapterKeys[newChapterIndex], newPage);
    }
}
