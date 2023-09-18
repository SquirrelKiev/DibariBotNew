using DibariBot.Database;
using DibariBot.Modules.Common;

namespace DibariBot.Modules.Manga;

public class MangaModule : DibariModule
{
    private readonly MangaService mangaHandler;
    private readonly DbService dbService;

    public MangaModule(MangaService mangaHandler, DbService db)
    {
        this.mangaHandler = mangaHandler;
        dbService = db;
    }

    [SlashCommand("manga", "Gets a page from a chapter of a manga.")]
    public async Task MangaSlash(string url = "", string chapter = "", int page = 1)
    {
        await DeferAsync();

        if (string.IsNullOrWhiteSpace(url))
        {
            using var context = dbService.GetDbContext();

            var exists = context.DefaultMangas.FirstOrDefault(x => x.GuildId == Context.Guild.Id && x.ChannelId == Context.Channel.Id);
            exists ??= context.DefaultMangas.FirstOrDefault(x => x.GuildId == Context.Guild.Id && x.ChannelId == 0ul);

            if (exists == null)
            {
                await FollowupAsync(embed:
                    new EmbedBuilder()
                    .WithDescription("This server hasn't set a default manga yet! Please manually specify the URL.") // TODO: l18n
                    .Build()
                );
                return;
            }

            url = exists.Manga;
        }
        var series = ParseUrl.ParseMangaUrl(url);

        if (series == null)
        {
            await FollowupAsync(embed:
                new EmbedBuilder()
                .WithDescription("Unsupported/invalid URL. Please make sure you're using a link that is supported by the bot.") // TODO: l18n
                .Build()
            );
            return;
        }

        var state = new MangaService.State(MangaAction.Open, series.Value, new Bookmark(chapter, page - 1));

        var contents = await mangaHandler.GetNewMessageContents(state);

        await FollowupAsync(contents);
    }

    [ComponentInteraction($"{ModulePrefixes.MANGA_MODULE_PREFIX}*")]
    public async Task MangaComponentInteraction(string rawState)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<MangaService.State>(rawState);

        var contents = await mangaHandler.GetNewMessageContents(state);

        await ModifyOriginalResponseAsync(contents);
    }
}
