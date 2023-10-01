using DibariBot.Core.Database.Models;
using DibariBot.Database;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Modules.Manga;

public enum MangaAction
{
    Open,
    BackPage,
    ForwardPage,
    BackChapter,
    ForwardChapter,
}

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class MangaService
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
    private readonly BotConfig config;
    private readonly DbService dbService;

    public MangaService(MangaFactory mangaFactory, BotConfig botConfig, DbService dbService)
    {
        this.mangaFactory = mangaFactory;
        this.config = botConfig;
        this.dbService = dbService;
    }

    public async Task<MessageContents> MangaCommand(ulong guildId, ulong channelId, string url = "", string chapter = "", int page = 1)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            await using var context = dbService.GetDbContext();
            
            var exists = context.DefaultMangas.FirstOrDefault(x => x.GuildId == guildId && x.ChannelId == channelId);
            exists ??= context.DefaultMangas.FirstOrDefault(x => x.GuildId == guildId && x.ChannelId == 0ul);

            if (exists == null)
            {
                return new MessageContents(string.Empty, embed:
                    new EmbedBuilder()
                    .WithDescription("This server hasn't set a default manga! Please manually specify the URL.") // TODO: l18n
                    .Build(), null);
            }

            url = exists.Manga;
        }
        var series = ParseUrl.ParseMangaUrl(url);

        if (series == null)
        {
            return new MessageContents(string.Empty, embed:
                new EmbedBuilder()
                .WithDescription("Unsupported/invalid URL. Please make sure you're using a link that is supported by the bot.") // TODO: l18n
                .Build(), null);
        }

        var state = new State(MangaAction.Open, series.Value, new Bookmark(chapter, page - 1));

        var contents = await GetMangaMessage(state);

        return contents;
    }

    public async Task<MessageContents> GetMangaMessage(State state)
    {
        IManga manga;
        try
        {
            manga = await mangaFactory.GetManga(state.identifier) ?? throw new NotImplementedException($"Platform \"{state.identifier.platform}\" not implemented!");
        }
        catch (HttpRequestException ex)
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription($"Failed to get manga. `{ex.Message}`\n `{state.identifier.platform}/{state.identifier.series}`")
                .WithColor(config)
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        var bookmark = new Bookmark(
            state.bookmark.chapter == "" ? await manga.DefaultChapter() : state.bookmark.chapter,
            state.bookmark.page);

        if (!await manga.HasChapter(bookmark.chapter))
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription("Chapter not found.")
                .WithColor(config)
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
                .WithColor(config)
                .Build();

            return new MessageContents(string.Empty, errorEmbed, null);
        }

        var pageSrc = GetProxiedUrl(pages.srcs[bookmark.page], state.identifier.platform!);

        var metadata = await manga.GetMetadata();

        string author = metadata.author;

        bool disableLeftChapter = await manga.GetPreviousChapterKey(bookmark.chapter) == null;
        bool disableRightChapter = await manga.GetNextChapterKey(bookmark.chapter) == null;

        bool disableLeftPage = disableLeftChapter && bookmark.page <= 0;
        bool disableRightPage = disableRightChapter && bookmark.page >= pages.srcs.Length;


        var embed = new EmbedBuilder()
            .WithTitle(string.IsNullOrWhiteSpace(chapterData.title) ? $"Chapter {bookmark.chapter}" : chapterData.title)
            .WithUrl(manga.GetUrl(bookmark))
            .WithDescription($"Chapter {bookmark.chapter} | Page {bookmark.page + 1}/{pages.srcs.Length}")
            .WithImageUrl(pageSrc)
            .WithFooter(
                new EmbedFooterBuilder()
                .WithText($"{metadata.title.Truncate(50, true)}, by {author}.\n" +
                $"Group: {pages.group}")
            )
            .WithColor(config)
            .Build();

        var newState = new State(MangaAction.Open, state.identifier, bookmark);

        var components = new ComponentBuilder()
            .WithButton(
                "<<",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackChapter),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftChapter,
                style: config.PrimaryButtonStyle
                )
            .WithButton(
                "<",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.BackPage),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableLeftPage,
                style: config.PrimaryButtonStyle
                )
            .WithButton(
                ">",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardPage),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightPage,
                style: config.PrimaryButtonStyle
                )
            .WithButton(
                ">>",
                StateSerializer.SerializeObject(newState.WithAction(MangaAction.ForwardChapter),
                ModulePrefixes.MANGA_MODULE_PREFIX),
                disabled: disableRightChapter,
                style: config.PrimaryButtonStyle
                )
            .WithRedButton();

        return new MessageContents(string.Empty, embed, components);
    }

    /// <remarks>
    /// Will only return a proxied url if told to by <see cref="BotConfig"/>.
    /// </remarks>
    private string GetProxiedUrl(string url, string platform)
    {
        if (!config.PlatformsToProxy.Contains(platform.ToLower()))
            return url;

        return config.ProxyUrlEncoding switch
        {
            BotConfig.ProxyUrlEncodingFormat.UrlEscaped => config.ProxyUrl.Replace("{{URL}}", System.Web.HttpUtility.UrlEncode(url)),
            BotConfig.ProxyUrlEncodingFormat.Base64 => config.ProxyUrl.Replace("{{URL}}", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url))),
            _ => throw new NotSupportedException(),
        };
    }

    // Regex filter zone

    /// <param name="filter"></param>
    /// <returns>The ID of the entry added/updated. If the ID is zero, it means the ID could not be found.</returns>
    // TODO: reasonable limits on how many regexes one server can have
    public async Task<uint> UpdateOrAddRegexFilter(RegexFilter filter)
    {
        await using var context = dbService.GetDbContext();

        if (filter.Id != 0ul)
        {
            // exists?
            var potential = await context.RegexFilters.FindAsync(filter.Id);

            if (potential == null)
            {
                return 0;
            }

            context.Entry(potential).CurrentValues.SetValues(filter);

            await context.SaveChangesAsync();

            return filter.Id;
        }
        else
        {
            var res = context.Add(filter);

            await context.SaveChangesAsync();

            return res.Entity.Id;
        }
    }

    public async Task<RegexFilter?> GetFilter(uint regexKey)
    {
        await using var context = dbService.GetDbContext();

        return await context.RegexFilters.FindAsync(regexKey);
    }

    /// <param name="guildId">ID of the guild.</param>
    /// <param name="channelId">Channel ID. will only grab the filters that apply to the current channel.</param>
    public async Task<RegexFilter[]> GetFilters(ulong guildId, ulong channelId)
    {
        await using var context = dbService.GetDbContext();

        var filterQuery = context.RegexFilters
            .Where(rf =>
                // The filter is for the current guild
                rf.GuildId == guildId
                && // AND
                (
                    // If the filter's scope is Include, check if there's an entry for the current channel.
                    (rf.ChannelFilterScope == ChannelFilterScope.Include && rf.RegexChannelEntries.Any(rce => rce.ChannelId == channelId))
                    || // OR
                    // If the filter's scope is Exclude, check if there isn't an entry for the current channel.
                    (rf.ChannelFilterScope == ChannelFilterScope.Exclude && rf.RegexChannelEntries.All(rce => rce.ChannelId != channelId))
                )
            );

        // Log.Verbose("GetFilter query is: {query}", filterQuery.ToQueryString());

        return await filterQuery.ToArrayAsync();
    }

    public async Task<RegexFilter[]> GetFilters(ulong guildId)
    {
        await using var context = dbService.GetDbContext();

        var guildFilters = await context.RegexFilters
            .Where(x => x.GuildId == guildId)
            .Include(x => x.RegexChannelEntries).ToArrayAsync();

        return guildFilters;
    }
}
