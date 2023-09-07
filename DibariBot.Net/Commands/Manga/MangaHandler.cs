using Discord;

namespace DibariBot.Commands.Manga;

public enum MangaAction
{
    Open,
    BackPage,
    ForwardPage,
    BackChapter,
    ForwardChapter,
}

public class MangaHandler
{
    public struct State
    {
        public MangaAction action = MangaAction.Open;
        public SeriesIdentifier identifier = new();
        public Bookmark bookmark = new();

        public State() { }

        public State(MangaAction interactionType, SeriesIdentifier identifier, Bookmark bookmark)
        {
            this.action = interactionType;
            this.identifier = identifier;
            this.bookmark = bookmark;
        }

        public State(MangaAction interactionType, string? platform, string? series, string chapter, int page)
        {
            this.action = interactionType;
            identifier = new(platform, series);
            bookmark = new(chapter, page);
        }

        public State WithAction(MangaAction interactionType)
        {
            this.action = interactionType;

            return this;
        }
    }

    private readonly CubariApi cubariApi;

    public MangaHandler(CubariApi cubariApi)
    {
        this.cubariApi = cubariApi;
    }

    public async Task<MessageContents> GetNewMessageContents(State state)
    {
        var manga = await cubariApi.GetManga(state.identifier);

        var bookmark = new Bookmark(
            state.bookmark.chapter ?? manga.chapters.First().Key,
            state.bookmark.page);

        if (!manga.chapters.ContainsKey(bookmark.chapter!))
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription("Chapter not found.")
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        switch (state.action)
        {
            case MangaAction.Open:
                break;
            case MangaAction.BackPage:
                bookmark = await cubariApi.Navigate(manga, bookmark, 0, -1);
                break;
            case MangaAction.ForwardPage:
                bookmark = await cubariApi.Navigate(manga, bookmark, 0, 1);
                break;
            case MangaAction.BackChapter:
                bookmark = await cubariApi.Navigate(manga, bookmark, -1, 0);
                break;
            case MangaAction.ForwardChapter:
                bookmark = await cubariApi.Navigate(manga, bookmark, 1, 0);
                break;
            default:
                throw new NotImplementedException($"{nameof(state.action)} not implemented!");
        }

        var pages = await cubariApi.GetImageSrcs(manga, bookmark.chapter!);

        if (bookmark.page > pages.srcs.Length - 1 || bookmark.page < 0)
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription($"page {bookmark.page + 1} doesn't exist in chapter {bookmark.chapter}!")
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        var pageSrc = pages.srcs[bookmark.page];

        // TODO: Proxy image implementation

        var author = manga.Author;
        // TODO: Mangadex author grabbing

        var disableLeftChapter = manga.chapters.First().Key == bookmark.chapter;
        var disableRightChapter = manga.chapters.Last().Key == bookmark.chapter;

        var disableLeftPage = disableLeftChapter && bookmark.page <= 0;
        var disableRightPage = disableRightChapter && bookmark.page >= pages.srcs.Length;

        var chapterData = manga.chapters[bookmark.chapter!];

        var embed = new EmbedBuilder()
            .WithTitle(chapterData.title ?? $"Chapter {bookmark.chapter}")
            .WithUrl(cubariApi.GetUrl(manga, bookmark))
            .WithDescription($"Chapter {bookmark.chapter} | Page {bookmark.page + 1}/{pages.srcs.Length}")
            .WithImageUrl(pageSrc)
            .WithFooter(
                new EmbedFooterBuilder()
                .WithText($"{manga.Title.Truncate(50, true)}, by {author}.\n" +
                $"Group: {pages.group}")
            )
            .Build();

        var newState = new State(MangaAction.Open, state.identifier, bookmark);

        var components = new ComponentBuilder()
            .WithButton(
                "<<", 
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackChapter), 
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftChapter)
            .WithButton(
                "<", 
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackPage), 
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftPage)
            .WithButton(
                ">", 
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardPage), 
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightPage)
            .WithButton(
                ">>", 
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardChapter), 
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightChapter)
            .Build();

        return new MessageContents(string.Empty, embed, components);
    }
}
