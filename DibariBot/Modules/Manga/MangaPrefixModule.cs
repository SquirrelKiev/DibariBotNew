using Discord.Commands;

namespace DibariBot.Modules.Manga;

public class MangaPrefixModule : DibariPrefixModule
{
    private readonly MangaService mangaHandler;

    public MangaPrefixModule(MangaService mangaHandler)
    {
        this.mangaHandler = mangaHandler;
    }

    [Command("manga")]
    public Task MangaCommand(string url, string chapter, int page) => MangaCommandImpl(url, chapter, page);

    [Command("manga")]
    public Task MangaCommand(string chapter, int page) => MangaCommandImpl(chapter: chapter, page: page);

    [Command("manga")]
    public Task MangaCommand(string chapter) => MangaCommandImpl(chapter: chapter);

    [Command("manga")]
    public Task MangaCommand() => MangaCommandImpl();

    private async Task MangaCommandImpl(string url = "", string chapter = "", int page = 1)
    {
        await DeferAsync();

        var contents = await mangaHandler.MangaCommand(Context.Guild?.Id ?? 0ul, GetParentChannel().Id,
            url, chapter, page);

        await ReplyAsync(contents);
    }
}