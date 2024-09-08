using DibariBot.Database;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Modules.Manga;

public class MangaPrefixModule(MangaService mangaHandler, DbService dbService) : PrefixModule
{
    [NamedArgumentType]
    public class NameableArguments
    {
        public string Url { get; set; } = "";
        public string Ch { get; set; } = "";
        public int Pg { get; set; } = 1;
        public bool Spoiler { get; set; } = false;
    }

    [Command("manga"), Priority(20)]
    [Alias("m")]
    [ParentModulePrefix(typeof(MangaModule))]
    public Task MangaCommand(string url, string chapter, int page) => MangaCommandImpl(url, chapter, page);

    [Command("manga"), Priority(15)]
    [Alias("m")]
    [ParentModulePrefix(typeof(MangaModule))]
    public Task MangaCommand(string chapter, int page) => MangaCommandImpl(chapter: chapter, page: page);

    [Command("manga"), Priority(10)]
    [Alias("m")]
    [ParentModulePrefix(typeof(MangaModule))]
    public Task MangaCommand(string chapter) => MangaCommandImpl(chapter: chapter);

    [Command("manga"), Priority(5)]
    [Alias("m")]
    [ParentModulePrefix(typeof(MangaModule))]
    public Task MangaCommand(NameableArguments namedArgs) => MangaCommandImpl(namedArgs.Url, namedArgs.Ch, namedArgs.Pg, namedArgs.Spoiler);

    private async Task MangaCommandImpl(string url = "", string chapter = "", int page = 1, bool spoiler = false)
    {
        await DeferAsync();

        if (Context.Guild != null)
        {
            await using var context = dbService.GetDbContext();

            var config = await context.GetGuildConfig(Context.Guild.Id);

            var commandName = CommandHandler.GetCommandName(Context.Message, config.Prefix.Length);

            if (commandName == "m") commandName = "manga";

            var alias = await context.MangaCommandAliases.FirstOrDefaultAsync(x =>
                x.GuildId == Context.Guild.Id && x.SlashCommandName == commandName);

            if (alias != null)
            {
                url = alias.Manga;
            }
        }

        var contents = await mangaHandler.MangaCommand(Context.Guild?.Id ?? 0ul, GetParentChannel().Id,
            url, chapter, page, isSpoiler: spoiler);

        await ReplyAsync(contents);
    }
}
