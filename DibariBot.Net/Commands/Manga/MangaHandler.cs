﻿using DibariBot.Mangas;
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

    private readonly MangaFactory mangaFactory;

    public MangaHandler(MangaFactory mangaFactory)
    {
        this.mangaFactory = mangaFactory;
    }

    public async Task<MessageContents> GetNewMessageContents(State state)
    {
        var manga = await mangaFactory.GetManga(state.identifier) ?? throw new NotImplementedException($"Platform \"{state.identifier.platform}\" not implemented!");

        var bookmark = new Bookmark(
            state.bookmark.chapter ?? await manga.DefaultChapter(),
            state.bookmark.page);

        if (!await manga.HasChapter(bookmark.chapter))
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription("Chapter not found.")
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        var chapterData = await manga.GetChapterMetadata(bookmark.chapter);

        bookmark.chapter = chapterData.id;

        switch (state.action)
        {
            case MangaAction.Open:
                break;
            case MangaAction.BackPage:
                bookmark = await MangaNavigation.Navigate(manga, bookmark, 0, -1);
                break;
            case MangaAction.ForwardPage:
                bookmark = await MangaNavigation.Navigate(manga, bookmark, 0, 1);
                break;
            case MangaAction.BackChapter:
                bookmark = await MangaNavigation.Navigate(manga, bookmark, -1, 0);
                break;
            case MangaAction.ForwardChapter:
                bookmark = await MangaNavigation.Navigate(manga, bookmark, 1, 0);
                break;
            default:
                throw new NotImplementedException($"{nameof(state.action)} not implemented!");
        }

        chapterData = await manga.GetChapterMetadata(bookmark.chapter);

        bookmark.chapter = chapterData.id;

        var pages = await manga.GetImageSrcs(bookmark.chapter);

        if (bookmark.page > pages.srcs.Length - 1 || bookmark.page < 0)
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription($"page {bookmark.page + 1} doesn't exist in chapter {bookmark.chapter}!")
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        var pageSrc = pages.srcs[bookmark.page];

        // TODO: Proxy image implementation

        var metadata = await manga.GetMetadata();
        
        string author = metadata.author;
        // TODO: Mangadex author grabbing

        bool disableLeftChapter = await manga.GetPreviousChapterKey(bookmark.chapter) == null;
        bool disableRightChapter = await manga.GetNextChapterKey(bookmark.chapter) == null;

        bool disableLeftPage = disableLeftChapter && bookmark.page <= 0;
        bool disableRightPage = disableRightChapter && bookmark.page >= pages.srcs.Length;


        var embed = new EmbedBuilder()
            .WithTitle(chapterData.title ?? $"Chapter {bookmark.chapter}")
            .WithUrl(manga.GetUrl(bookmark))
            .WithDescription($"Chapter {bookmark.chapter} | Page {bookmark.page + 1}/{pages.srcs.Length}")
            .WithImageUrl(pageSrc)
            .WithFooter(
                new EmbedFooterBuilder()
                .WithText($"{metadata.title.Truncate(50, true)}, by {author}.\n" +
                $"Group: {pages.group}")
            )
            .Build();

        var newState = new State(MangaAction.Open, state.identifier, bookmark);

        var components = new ComponentBuilder()
            .WithButton(
                "<<",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackChapter),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftChapter
                )
            .WithButton(
                "<",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackPage),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftPage
                )
            .WithButton(
                ">",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardPage),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightPage
                )
            .WithButton(
                ">>",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardChapter),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightChapter
                )
            .Build();

        return new MessageContents(string.Empty, embed, components);
    }
}
