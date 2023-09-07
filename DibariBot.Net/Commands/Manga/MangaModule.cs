using Discord;
using Discord.Interactions;

namespace DibariBot.Commands.Manga;

public class MangaModule : DibariModule
{
    private readonly MangaHandler mangaHandler;

    public MangaModule(MangaHandler mangaHandler)
    {
        this.mangaHandler = mangaHandler;
    }

    [SlashCommand("manga", "Gets a page from a chapter of a manga.")]
    public async Task MangaSlash(string url, string chapter = "1", int page = 1)
    {
        await DeferAsync();

        var series = ParseUrl.ParseMangaUrl(url);

        if (series == null)
        {
            await FollowupAsync(embed:
                new EmbedBuilder()
                .WithDescription("Unsupported/invalid URL. Please make sure you're using a link that is supported by Cubari.")
                .Build()
            );
            return;
        }

        var state = new MangaHandler.State(MangaAction.Open, series.Value, new Bookmark(chapter, page - 1));

        var contents = await mangaHandler.GetNewMessageContents(state);

        await FollowupAsync(contents);
    }

    [ComponentInteraction($"{ModulePrefixes.MANGA_MODULE_PREFIX}*")]
    public async Task MangaComponentInteraction(string rawState)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<MangaHandler.State>(rawState);

        var contents = await mangaHandler.GetNewMessageContents(state);

        await ModifyOriginalResponseAsync(contents);
    }
}
