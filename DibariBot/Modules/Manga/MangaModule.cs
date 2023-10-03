using Discord.WebSocket;

namespace DibariBot.Modules.Manga;

public class MangaModule : DibariModule
{
    private readonly MangaService mangaHandler;

    public MangaModule(MangaService mangaHandler)
    {
        this.mangaHandler = mangaHandler;
    }

    [SlashCommand("manga", "Gets a page from a chapter of a manga.")]
    [EnabledInDm(true)]
    public async Task MangaSlash(string url = "", string chapter = "", int page = 1)
    {
        await DeferAsync();

        var contents = await mangaHandler.MangaCommand(Context.Guild?.Id ?? 0ul, GetParentChannel().Id,
            url, chapter, page);

        await FollowupAsync(contents);
    }

    [ComponentInteraction($"{ModulePrefixes.MANGA_MODULE_PREFIX}*")]
    public async Task MangaComponentInteraction(string rawState)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<MangaService.State>(rawState);

        var contents = await mangaHandler.GetMangaMessage(Context.Guild?.Id ?? 0ul, GetParentChannel().Id, state);

        await ModifyOriginalResponseAsync(contents);
    }

    
}
