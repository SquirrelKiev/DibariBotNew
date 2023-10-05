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
    public async Task MangaSlash(string url = "", string chapter = "", int page = 1, bool ephemeral = false)
    {
        await DeferAsync(ephemeral);

        var contents = await mangaHandler.MangaCommand(Context.Guild?.Id ?? 0ul, GetParentChannel().Id,
            url, chapter, page, ephemeral);

        await FollowupAsync(contents, ephemeral);
    }

    [ComponentInteraction($"{ModulePrefixes.MANGA_MODULE_PREFIX}*")]
    public async Task MangaComponentInteraction(string rawState)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<MangaService.State>(rawState);

        var ogRes = await GetOriginalResponseAsync();

        var shouldResend = state.action == MangaAction.SendNonEphemeral;
        var isEphemeral = (ogRes.Flags & MessageFlags.Ephemeral) != 0 && !shouldResend;

        var contents =
            await mangaHandler.GetMangaMessage(Context.Guild?.Id ?? 0ul, GetParentChannel().Id, state, isEphemeral);

        if (shouldResend)
        {
            await FollowupAsync(contents);
            await Context.Interaction.DeleteOriginalResponseAsync();
        }
        else
        {
            await ModifyOriginalResponseAsync(contents);
        }
    }
}