namespace DibariBot.Modules.Manga;

public static class MangaNavigation
{
    public static async Task<Bookmark> Navigate(IManga manga, Bookmark currentLocation, int deltaChapters, int deltaPages)
    {
        string currentChapterKey = currentLocation.chapter;
        
        // Adjust the chapter based on deltaChapters.
        for (int i = 0; i < Math.Abs(deltaChapters); i++)
        {
            string? nextChapterKey = deltaChapters > 0 ? await manga.GetNextChapterKey(currentChapterKey) : await manga.GetPreviousChapterKey(currentChapterKey);
            if (nextChapterKey != null)
            {
                currentChapterKey = nextChapterKey;
            }
            else
            {
                // Reached the beginning or the end of the manga; stop adjusting chapters.
                break;
            }
        }

        // If chapter changed, reset page to the start of the new chapter.
        int newPage = (deltaChapters != 0) ? 0 : currentLocation.page;

        // Adjust page based on deltaPages within a loop to handle cross-chapter navigation.
        while (deltaPages != 0)
        {
            var pageUrls = await manga.GetImageSrcs(currentChapterKey);

            if (deltaPages > 0)
            {
                // Moving forward
                if (newPage + deltaPages < pageUrls.srcs.Length)
                {
                    newPage += deltaPages;
                    deltaPages = 0;
                }
                else
                {
                    deltaPages -= (pageUrls.srcs.Length - newPage);
                    newPage = 0;
                    string? nextChapterKey = await manga.GetNextChapterKey(currentChapterKey);
                    if (nextChapterKey != null)
                    {
                        currentChapterKey = nextChapterKey;
                    }
                    else
                    {
                        // Reached the end of the manga; stop adjusting pages.
                        break;
                    }
                }
            }
            else
            {
                // Moving backward
                if (newPage + deltaPages >= 0)
                {
                    newPage += deltaPages;
                    deltaPages = 0;
                }
                else
                {
                    deltaPages += newPage + 1; // Adjusting for zero-based indexing.
                    string? previousChapterKey = await manga.GetPreviousChapterKey(currentChapterKey);
                    if (previousChapterKey != null)
                    {
                        currentChapterKey = previousChapterKey;
                        pageUrls = await manga.GetImageSrcs(currentChapterKey);
                        newPage = pageUrls.srcs.Length - 1;
                    }
                    else
                    {
                        // Reached the beginning of the manga; stop adjusting pages.
                        newPage = 0;
                        deltaPages = 0;
                    }
                }
            }
        }

        return new Bookmark { chapter = currentChapterKey, page = newPage };
    }

}
