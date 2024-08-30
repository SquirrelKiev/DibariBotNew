using Discord.Commands;

namespace DibariBot.Modules.MDSearch;

public class SearchPrefixModule(SearchService search) : PrefixModule
{
    [NamedArgumentType]
    public class NameableArguments
    {
        public bool Spoiler { get; set; } = false;

    }

    [Command("search")]
    [ParentModulePrefix(typeof(SearchModule))]
    public async Task SearchCommand(string query, NameableArguments args)
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        await ReplyAsync(await search.GetMessageContents(new SearchService.State { query = query, isSpoiler = args.Spoiler }, Context.Guild));
    }

    [Command("search")]
    [ParentModulePrefix(typeof(SearchModule))]
    public async Task SearchCommand([Remainder] string query)
    {
        // oshi no ko, super lazy """fix"""
        if (Context.Guild != null && Context.Guild.Id == 695200821910044783ul)
            return;

        await DeferAsync();

        await ReplyAsync(await search.GetMessageContents(new SearchService.State { query = query, isSpoiler = false }, Context.Guild));
    }
}